using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HotelOS.WebsocketService.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WebsocketDbContext>();
        
        try
        {
            var connection = context.Database.GetDbConnection() as NpgsqlConnection;
            if (connection != null)
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    await connection.OpenAsync();
                
                // Create schema if not exists
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "CREATE SCHEMA IF NOT EXISTS websocket";
                    await cmd.ExecuteNonQueryAsync();
                }
                
                // Create notifications table if not exists
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS websocket.notifications (
    id SERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    title VARCHAR(200) NOT NULL,
    message VARCHAR(500) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    is_read BOOLEAN NOT NULL DEFAULT false,
    target_role VARCHAR(50)
);

CREATE INDEX IF NOT EXISTS ix_notifications_created_at ON websocket.notifications(created_at);
";
                    await cmd.ExecuteNonQueryAsync();
                }
                
                // Insert seed data if table is empty
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
INSERT INTO websocket.notifications (type, title, message, created_at, is_read, target_role)
SELECT 'reception', 'Guest checked in', 'Room assignment completed for Amelia Stone.', '2026-05-20 09:12:00+00', false, null
WHERE NOT EXISTS (SELECT 1 FROM websocket.notifications WHERE id = 1);

INSERT INTO websocket.notifications (type, title, message, created_at, is_read, target_role)
SELECT 'maintenance', 'Maintenance escalated', 'Room 302 moved to critical priority.', '2026-05-20 09:05:00+00', false, null
WHERE NOT EXISTS (SELECT 1 FROM websocket.notifications WHERE id = 2);
";
                    await cmd.ExecuteNonQueryAsync();
                }
                
                await connection.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not initialize database: {ex.Message}");
            throw;
        }
    }
}