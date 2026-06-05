using Microsoft.Extensions.Logging;

namespace HotelOS.Shared.Startup;

public static class StartupTaskRunner
{
    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30)
    ];

    public static async Task RunAsync(
        string operationName,
        Func<Task> operation,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var attempt = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await operation();

                if (attempt > 0)
                {
                    logger.LogInformation("{Operation} succeeded after {Attempts} attempt(s)", operationName, attempt + 1);
                }

                return;
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                var delay = RetryDelays[Math.Min(attempt, RetryDelays.Length - 1)];
                attempt++;

                logger.LogWarning(
                    exception,
                    "{Operation} failed. Retrying in {DelaySeconds}s (attempt {Attempt})",
                    operationName,
                    (int)delay.TotalSeconds,
                    attempt);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}