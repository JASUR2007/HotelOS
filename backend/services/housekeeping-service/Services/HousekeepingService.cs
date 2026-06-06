using HotelOS.HousekeepingService.DTOs;
using HotelOS.HousekeepingService.Events;
using HotelOS.HousekeepingService.Models;
using HotelOS.HousekeepingService.Repositories;
using HotelOS.Shared.Constants;
using HotelOS.Shared.RabbitMQ;

namespace HotelOS.HousekeepingService.Services;

public sealed class HousekeepingService(IHousekeepingRepository repository, IEventPublisher eventPublisher) : IHousekeepingService
{
    public async Task<IReadOnlyList<CleaningTaskDto>> GetQueueAsync(CancellationToken cancellationToken = default)
        => (await repository.GetQueueAsync(cancellationToken))
            .Select(task => new CleaningTaskDto(task.Id, task.RoomId, task.RoomNumber, task.Status, task.AssignedTo, task.Priority))
            .ToList();

    public async Task<CleaningTaskDto> UpdateStatusAsync(UpdateCleaningStatusDto request, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new InvalidOperationException("Cleaning task not found");

        var transition = $"{task.Status}->{request.Status}";
        if (!ValidTransitions.Contains(transition) && request.Status != task.Status)
        {
            throw new InvalidOperationException(
                $"Invalid status transition: {task.Status} → {request.Status}. " +
                $"Valid transitions: {string.Join(", ", ValidTransitions)}.");
        }

        task.Status = request.Status;
        task.AssignedTo = request.AssignedTo;
        await repository.SaveChangesAsync(cancellationToken);

        if (request.Status.Contains("Complete", StringComparison.OrdinalIgnoreCase))
        {
            eventPublisher.Publish(RabbitMqRoutingKeys.RoomCleaned, new
            {
                task.RoomId,
                task.RoomNumber,
                task.Status,
                task.AssignedTo,
                OccurredAt = DateTimeOffset.UtcNow
            });

            _ = new CleaningCompletedEvent(task.RoomId, task.RoomNumber, DateTimeOffset.UtcNow);
        }
        else
        {
            eventPublisher.Publish(RabbitMqRoutingKeys.RoomVacated, new
            {
                task.RoomId,
                task.RoomNumber,
                task.Status,
                OccurredAt = DateTimeOffset.UtcNow
            });

            _ = new CleaningQueuedEvent(task.RoomId, task.RoomNumber, DateTimeOffset.UtcNow);
        }

        return new CleaningTaskDto(task.Id, task.RoomId, task.RoomNumber, task.Status, task.AssignedTo, task.Priority);
    }

    public async Task<CleaningTaskDto> CreateTaskAsync(CreateCleaningTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await repository.AddAsync(new CleaningTask
        {
            RoomId = new Random().Next(100, 999),
            RoomNumber = request.RoomNumber,
            Status = "Queued",
            AssignedTo = request.AssignedTo,
            Priority = request.Priority
        }, cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.RoomVacated, new
        {
            task.RoomId,
            task.RoomNumber,
            task.Status,
            task.Priority,
            OccurredAt = DateTimeOffset.UtcNow
        });

        _ = new CleaningQueuedEvent(task.RoomId, task.RoomNumber, DateTimeOffset.UtcNow);

        return new CleaningTaskDto(task.Id, task.RoomId, task.RoomNumber, task.Status, task.AssignedTo, task.Priority);
    }

    private static readonly HashSet<string> ValidTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Queued->InProgress",
        "InProgress->Completed"
    };

    public async Task<CleaningTaskDto> UpdateTaskAsync(int id, UpdateCleaningTaskDto request, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Cleaning task not found");

        if (request.Status is not null)
        {
            var transition = $"{task.Status}->{request.Status}";
            if (!ValidTransitions.Contains(transition) && request.Status != task.Status)
            {
                throw new InvalidOperationException(
                    $"Invalid status transition: {task.Status} → {request.Status}. " +
                    $"Valid transitions: {string.Join(", ", ValidTransitions)}.");
            }
            task.Status = request.Status;
        }
        if (request.AssignedTo is not null)
            task.AssignedTo = request.AssignedTo;

        await repository.SaveChangesAsync(cancellationToken);

        if (request.Status is not null && request.Status.Contains("Complete", StringComparison.OrdinalIgnoreCase))
        {
            eventPublisher.Publish(RabbitMqRoutingKeys.RoomCleaned, new
            {
                task.RoomId,
                task.RoomNumber,
                task.Status,
                task.AssignedTo,
                OccurredAt = DateTimeOffset.UtcNow
            });

            _ = new CleaningCompletedEvent(task.RoomId, task.RoomNumber, DateTimeOffset.UtcNow);
        }
        else
        {
            eventPublisher.Publish(RabbitMqRoutingKeys.RoomVacated, new
            {
                task.RoomId,
                task.RoomNumber,
                task.Status,
                task.AssignedTo,
                OccurredAt = DateTimeOffset.UtcNow
            });

            _ = new CleaningQueuedEvent(task.RoomId, task.RoomNumber, DateTimeOffset.UtcNow);
        }

        return new CleaningTaskDto(task.Id, task.RoomId, task.RoomNumber, task.Status, task.AssignedTo, task.Priority);
    }

    public async Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Cleaning task not found");

        await repository.DeleteAsync(task, cancellationToken);

        eventPublisher.Publish(RabbitMqRoutingKeys.RoomVacated, new
        {
            task.RoomId,
            task.RoomNumber,
            Status = "Deleted",
            OccurredAt = DateTimeOffset.UtcNow
        });
    }
}
