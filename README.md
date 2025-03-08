Rate Limiter API

Overview

This project implements a simple rate-limiting mechanism for an API using .NET. It ensures that users can only make a specified number of requests within a given time window.

Technologies Used

.NET 6+

ASP.NET Core

C#

Concurrent Collections

Dependency Injection

Logging

Features

Limits users to a maximum of 5 requests within a 10-second window.

Implements a rate limiter service that tracks requests per user.

Provides API endpoints to test request allowance and view user request logs.

Project Structure

Rate_LimiterExercise
│── Controllers
│   ├── DummyController.cs
│── Services
│   ├── Interfaces
│   │   ├── IRateLimiter.cs
│   ├── Classes
│   │   ├── RateLimiter.cs

Installation & Setup

Clone the repository:

git clone https://github.com/yourusername/Rate_LimiterExercise.git

Navigate to the project folder:

cd Rate_LimiterExercise

Build and run the project:

dotnet run

API Endpoints

1. Simulate Requests

Endpoint: GET /api/Dummy/AllowRequest

This endpoint simulates multiple users sending requests to test the rate-limiting mechanism. Logs will indicate whether requests are allowed or denied.

2. Get Current Requests

Endpoint: GET /api/Dummy/GetRequests

Returns a list of users along with their recorded request timestamps.

Implementation Details

Rate Limiter Service

public class RateLimiter : IRateLimiter
{
    public ConcurrentDictionary<string, List<DateTime>> RateLimit { get; } = new();
    private readonly TimeSpan _timeWindow = TimeSpan.FromSeconds(10);
    private readonly int _maxRequests = 5;

    public bool AllowRequests(string userId)
    {
        var now = DateTime.Now;
        RateLimit.TryAdd(userId, new List<DateTime> { now });
        Reset(userId);

        if (RateLimit[userId].Count >= _maxRequests)
            return false;

        RateLimit[userId].Add(now);
        return true;
    }

    public void Reset(string userId)
    {
        RateLimit[userId].RemoveAll(x => x + _timeWindow < DateTime.Now);
    }
}

Dummy Controller

[ApiController]
[Route("api/[controller]")]
public class DummyController : ControllerBase
{
    private readonly ILogger<DummyController> _logger;
    private readonly IRateLimiter _rateLimiter;
    private List<string> _users = new() { "userOne", "userTwo", "userThree" };

    public DummyController(ILogger<DummyController> logger, IRateLimiter rateLimiter)
    {
        _logger = logger;
        _rateLimiter = rateLimiter;
    }

    [HttpGet("AllowRequest")]
    public async Task<IActionResult> AllowRequest()
    {
        async Task SimulateRequests(string user, int delay)
        {
            for (int i = 0; i < 10; i++)
            {
                var isAllowed = _rateLimiter.AllowRequests(user);
                _logger.LogInformation($"User {user} request {i + 1}: {(isAllowed ? "Allowed" : "Denied")}");
                await Task.Delay(delay);
                delay += 500;
            }
        }

        var tasks = new List<Task>
        {
            SimulateRequests(_users[0], 0),
            SimulateRequests(_users[1], 1000),
            SimulateRequests(_users[2], 2000)
        };

        await Task.WhenAll(tasks);
        return Ok("Dummy request!");
    }

    [HttpGet("GetRequests")]
    public IActionResult GetRequests()
    {
        var requests = _rateLimiter.RateLimit;
        return Ok(requests.Select(x => $"User: {x.Key} TimeStamps: {string.Join(" ", x.Value)}"));
    }
}

Contributing

Feel free to fork the project and submit pull requests. Suggestions and improvements are welcome!
