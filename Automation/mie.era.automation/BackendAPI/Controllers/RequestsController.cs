using BackendAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly IRequests _requests;

        public RequestsController(IRequests requests)
        {
            _requests = requests;
                
        }

        [HttpGet]
        public string GetRequestStatus(string RequestKey)
        {
            return _requests.GetRequestStatus(RequestKey);
        }
    }
}
