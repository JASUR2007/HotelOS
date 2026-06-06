using HotelOS.MaintenanceService.DTOs;
using HotelOS.MaintenanceService.Events;
using HotelOS.MaintenanceService.Models;
using HotelOS.MaintenanceService.Repositories;
using HotelOS.Shared.Algorithms;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;
using HotelOS.Shared.Audit;

namespace HotelOS.MaintenanceService.Services;

public sealed class MaintenanceService(
    IMaintenanceRepository repository,
    IEventPublisher eventPublisher,
    IAuditLogger auditLogger,
    MaintenancePriorityQueue priorityQueue) : IMaintenanceService, IMaintenanceQueries, IMaintenanceCommands
{
    public async Task<IReadOnlyList<MaintenanceIssueDto>> GetIssuesAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken))
            .Select(issue => new MaintenanceIssueDto(issue.Id, issue.RoomNumber, issue.Title, issue.Category, issue.Priority, issue.Status, issue.TechnicianName))
            .ToList();

    public async Task<MaintenanceIssueDto> CreateIssueAsync(CreateIssueDto request, CancellationToken cancellationToken = default)
    {
        var issue = await repository.AddAsync(new MaintenanceIssue
        {
            RoomNumber = request.RoomNumber,
            Title = request.Title,
            Category = request.Category,
            Priority = request.Priority,
            Status = "Queued"
        }, cancellationToken);

        var priority = ParsePriority(request.Priority);
        priorityQueue.Enqueue(new MaintenanceQueueItem(issue.Id, issue.Title, priority, DateTimeOffset.UtcNow));

        eventPublisher.Publish(RabbitMqRoutingKeys.MaintenanceCreated, new
        {
            issue.Id,
            issue.RoomNumber,
            issue.Title,
            issue.Priority,
            issue.Status,
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Staff", "Created Maintenance", $"Issue #{issue.Id}", $"{issue.Title} (Room {issue.RoomNumber}, {issue.Priority})");

        _ = new MaintenanceIssueRaisedEvent(issue.Id, issue.RoomNumber, DateTimeOffset.UtcNow);
        return new MaintenanceIssueDto(issue.Id, issue.RoomNumber, issue.Title, issue.Category, issue.Priority, issue.Status, issue.TechnicianName);
    }

    public async Task<MaintenanceIssueDto> AssignTechnicianAsync(AssignTechnicianDto request, CancellationToken cancellationToken = default)
    {
        MaintenanceIssue? issue;

        if (request.IssueId == 0 && priorityQueue.TryDequeue(out var nextItem) && nextItem is not null)
        {
            issue = await repository.GetByIdAsync(nextItem.TicketId, cancellationToken);
        }
        else
        {
            issue = await repository.GetByIdAsync(request.IssueId, cancellationToken);
        }

        if (issue is null)
            throw new InvalidOperationException("Maintenance issue not found");

        issue.Status = "Assigned";
        await repository.SaveChangesAsync(cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.MaintenanceAssigned, new
        {
            issue.Id,
            issue.RoomNumber,
            issue.Title,
            TechnicianName = request.TechnicianName,
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Staff", "Assigned Technician", $"Issue #{issue.Id}", $"→ {request.TechnicianName}");

        _ = new MaintenanceIssueAssignedEvent(issue.Id, request.TechnicianName, DateTimeOffset.UtcNow);
        return new MaintenanceIssueDto(issue.Id, issue.RoomNumber, issue.Title, issue.Category, issue.Priority, issue.Status, request.TechnicianName);
    }

    public async Task<MaintenanceIssueDto> UpdateIssueAsync(int id, UpdateIssueDto request, CancellationToken cancellationToken = default)
    {
        var issue = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Maintenance issue not found");

        if (request.Status is not null)
            issue.Status = request.Status;
        if (request.TechnicianName is not null)
            issue.TechnicianName = request.TechnicianName;
        if (request.Priority is not null)
            issue.Priority = request.Priority;

        await repository.SaveChangesAsync(cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.MaintenanceCreated, new
        {
            issue.Id,
            issue.RoomNumber,
            issue.Title,
            issue.Priority,
            issue.Status,
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Staff", "Updated Maintenance", $"Issue #{issue.Id}", $"{issue.Title} (Status: {issue.Status}, Priority: {issue.Priority})");

        return new MaintenanceIssueDto(issue.Id, issue.RoomNumber, issue.Title, issue.Category, issue.Priority, issue.Status, issue.TechnicianName);
    }

    public async Task DeleteIssueAsync(int id, CancellationToken cancellationToken = default)
    {
        var issue = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Maintenance issue not found");

        await repository.DeleteAsync(issue, cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.MaintenanceCreated, new
        {
            issue.Id,
            issue.RoomNumber,
            issue.Title,
            Status = "Deleted",
            OccurredAt = DateTimeOffset.UtcNow
        });

        auditLogger.Log("Staff", "Deleted Maintenance", $"Issue #{issue.Id}", $"{issue.Title} (Room {issue.RoomNumber})");
    }

    private static MaintenancePriority ParsePriority(string priority) => priority switch
    {
        "Critical" => MaintenancePriority.Critical,
        "High" => MaintenancePriority.High,
        "Normal" => MaintenancePriority.Normal,
        "Low" => MaintenancePriority.Low,
        _ => MaintenancePriority.Normal
    };
}
