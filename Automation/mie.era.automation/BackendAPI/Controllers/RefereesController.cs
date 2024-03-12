using System.Collections.Generic;
using System.Formats.Asn1;
using System.Threading.Tasks;
using BackendAPI.Services;
using Common.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [MIEAuthorize]
    public class RefereesController : ControllerBase
    {
        private readonly RefereesService _refereesService;

        public RefereesController(RefereesService refereesService)
        {
            _refereesService = refereesService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RefereesService.RefereeInfo>>> GetRefereesWithCompletedReferences()
        {
            var referee = await _refereesService.GetRefereesWithCompletedReferencesAsync();
            return Ok(referee);
        }

        [HttpGet]
        public async Task<ActionResult<List<RefereesService.RefereeInfo>>> SearchReference(string filterInput)
        {
            var referee = await _refereesService.GetReferencesByFilterAsync(filterInput);
            
            return Ok(referee);
        }

    }
}