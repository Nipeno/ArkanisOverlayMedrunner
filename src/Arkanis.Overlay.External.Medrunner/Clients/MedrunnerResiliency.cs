namespace Arkanis.Overlay.External.Medrunner.Clients;

using System.Net;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

internal static class MedrunnerResiliency
{
    private static readonly RetryStrategyOptions RetryOptions = new()
    {
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        Delay = TimeSpan.FromSeconds(2),
        UseJitter = true,
        ShouldHandle = new PredicateBuilder()
            .Handle<HttpRequestException>()
            .Handle<TaskCanceledException>()
            .Handle<MedrunnerApiException>(ex => ex.StatusCode >= 500 || ex.StatusCode == (int)HttpStatusCode.TooManyRequests),
        DelayGenerator = args =>
        {
            // honour Retry-After on 429 if present
            if (args.Outcome.Exception is MedrunnerApiException { StatusCode: (int)HttpStatusCode.TooManyRequests, RetryAfter: { } retryAfter })
                return ValueTask.FromResult<TimeSpan?>(retryAfter);

            return ValueTask.FromResult<TimeSpan?>(null); // fall back to exponential backoff
        },
    };

    private static readonly CircuitBreakerStrategyOptions CircuitBreakerOptions = new()
    {
        // open circuit after 5 failures within a 30-second sampling window
        FailureRatio = 0.5,
        MinimumThroughput = 5,
        SamplingDuration = TimeSpan.FromSeconds(30),
        // stay open for 15 seconds before allowing a probe request through
        BreakDuration = TimeSpan.FromSeconds(15),
        ShouldHandle = new PredicateBuilder()
            .Handle<HttpRequestException>()
            .Handle<TaskCanceledException>()
            .Handle<MedrunnerApiException>(ex => ex.StatusCode >= 500),
    };

    public static readonly ResiliencePipeline Pipeline = new ResiliencePipelineBuilder()
        .AddRetry(RetryOptions)
        .AddCircuitBreaker(CircuitBreakerOptions)
        .Build();
}
