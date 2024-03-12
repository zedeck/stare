using BackendAPI.Services;
using Common.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [MIEAuthorize]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }


        // New method using StatisticsService to get completed references count
        [HttpGet]
        public async Task<ActionResult<int>> GetCompletedReferencesCount()
        {
            return Ok(await _statisticsService.GetCompletedReferencesCount());
        }


        // New method using StatisticsService to get average completion time
        [HttpGet]
        public async Task<ActionResult<double>> GetAverageCompletionTime()
        {
            return Ok(await _statisticsService.GetAverageCompletionTime());
        }

        // New method using StatisticsService to get average candidate score
        [HttpGet]
        public async Task<ActionResult<double>> GetAverageCandidateScore()
        {
            return Ok(await _statisticsService.GetAverageCandidateScore());
        }

        [HttpGet]
        public ActionResult<int> GetTotalReferencesCount()
        {
            var totalReferencesCount = _statisticsService.GetTotalReferencesCount();
            return Ok(totalReferencesCount);
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetNotificationCount(int refereeId)
        {
            return Ok(await _statisticsService.GetNotificationCount(refereeId));
        }

        [HttpGet]
        public async Task<ActionResult<DateTime?>> GetMaxNotificationDateTime(int refereeId)
        {
            return Ok(await _statisticsService.GetMaxNotificationDateTime(refereeId));
        }

    }
}
