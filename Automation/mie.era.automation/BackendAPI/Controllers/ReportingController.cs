using BackendAPI.Services;
using Common.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [MIEAuthorize]
    public class ReportingController : ControllerBase
    {
        //private readonly IConfiguration _configuration;
        //private readonly ERADBContext _context;
        private readonly ReportingService _reportingService;

        public ReportingController(ReportingService reportingSerivce)
        {
            _reportingService = reportingSerivce;
        }


        [HttpGet]
        public string Index()
        {
            //return View();
            return "This is the reporting controller";
        }

        [HttpPost]
        public async Task<IActionResult> SendAllResults()
        {

            AuthDetails details = HttpContext.GetAuthDetails();

            _reportingService.SubmitToPCV(details.Token);

            return Ok();
        }


    }
}
