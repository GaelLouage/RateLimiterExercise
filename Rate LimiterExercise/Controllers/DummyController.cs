using Microsoft.AspNetCore.Mvc;
using Rate_LimiterExercise.Services.Interfaces;
using System.Threading.RateLimiting;

namespace Rate_LimiterExercise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DummyController : ControllerBase
    {
        private readonly ILogger<DummyController> _logger;
        private List<string> _users = new List<string>()
        {
            "userOne",
            "userTwo",
            "userThree"
        };
        private readonly IRateLimiter _rateLimiter;

        public DummyController(ILogger<DummyController> logger, IRateLimiter rateLimiter)
        {
            _logger = logger;
            _rateLimiter = rateLimiter;
        }

        [HttpGet("AllowRequest")]
        public async Task<IActionResult> AllowRequest()
        {

            // dummy requests to simulate different requests at the same time
            async Task SimulateRequests(string user, int delay)
            {

                for (int i = 0; i < 10; i++) 
                {
                    var isAllowedRequest = _rateLimiter.AllowRequests(user);
                    _logger.LogInformation($"User {user} request {i + 1}: {(isAllowedRequest ? "Allowed" : "Denied")}");
                    await Task.Delay(delay);
                    delay += 500;
                }
            };


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
}
