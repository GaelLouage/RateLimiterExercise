using System.Collections.Concurrent;

namespace Rate_LimiterExercise.Services.Interfaces
{
    public interface IRateLimiter
    {
        ConcurrentDictionary<string, List<DateTime>> RateLimit { get; }
        bool AllowRequests(string userId);
        void Reset(string userId);
    }
}