using Microsoft.VisualBasic;
using Rate_LimiterExercise.Services.Interfaces;
using System.Collections.Concurrent;

namespace Rate_LimiterExercise.Services.Classes
{
    public class RateLimiter : IRateLimiter
    {
        public ConcurrentDictionary<string, List<DateTime>> RateLimit { get; } = new ConcurrentDictionary<string, List<DateTime>>();
        private readonly TimeSpan _timeWindow = TimeSpan.FromSeconds(10);
        private readonly int _maxRequests  = 5;
        public bool AllowRequests(string userId)
        {
            var now = DateTime.Now;

            // Add new user if not present
            RateLimit.TryAdd(userId, new List<DateTime>() { now});

            // Remove outdated timestamps
            Reset(userId);

            // check if all ratelimits cout are equals to the maxrequest
            if (RateLimit[userId].Count >= _maxRequests)
            {
                return false;
            }

            RateLimit[userId].Add(DateTime.Now);
            return true;
        }
        public void Reset(string userId)
        {
            RateLimit[userId].RemoveAll(x => x + _timeWindow < DateTime.Now);
        }
    }
}
