using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;

namespace mie.era.mvc.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HelperController : Controller
    {
        private readonly IConfiguration _config;

        public HelperController(IConfiguration config)
        {
            _config = config;  
        }


        [HttpGet]
        public string[] GetAnswerOptions(int questionID)
        {
            string[] answerOptions = { "opt1" + questionID , "opt2" + questionID, "opt3" + questionID, "opt4" + questionID };

            return answerOptions;
        }

    }
}
