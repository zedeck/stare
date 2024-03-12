using BackendAPI.Interfaces;
using BackendAPI.Models;
using Common.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;

namespace BackendAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [MIEAuthorize]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidate _candidateSrv;

        public CandidateController(ICandidate candidateSrv)
        {
            _candidateSrv = candidateSrv;
                
        }


        [HttpGet]
        public LCandidateModel GetCandidateDetails(string RemoteKey)
        {
           return _candidateSrv.GetCandidateInfo(RemoteKey);
        }

        [HttpGet]
        public LCandidateModel GetCandidateInfoByReqID(int RequestID)
        {
            return _candidateSrv.GetCandidateInfoByReqID(RequestID);
        }


        [HttpGet]
        public List<LCandidateListModel> GetListOfCandidates()
        {
            return _candidateSrv.GetListOfCandidates();
        }

        [HttpGet]
        public int GetCandidateCount()
        {
            return _candidateSrv.GetCandidateCount();
        }

        [HttpGet]
        public int GetTotalNumberOfReferences()
        {
            return _candidateSrv.GetTotalNumberOfReferences();
        }

        [HttpGet]
        public int GetNumberOfCompletedReferences()
        {
            return _candidateSrv.GetNumberOfCompletedReferences();
        }

        [HttpGet]
        public string GetReferenceLinkByRequestID(int RequestID)
        {
            return _candidateSrv.GetReferenceLinkByRequestID(RequestID);
        }


        //To change for reference
        [HttpGet]
        public ContentResult EditCandidateInfo(int RequestID) {


            string httpResp = "<a type=\"link\" data-bs-toggle=\"modal\" data-bs-target=\"#brokenLinkModal\" href=\"\">\r\n        ReferenceLink\r\n    </a>\r\n\r\n   \r\n    <div class=\"modal fade\" id=\"brokenLinkModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"brokenLinkModal\"\r\n        aria-hidden=\"false\">\r\n        <div class=\"modal-dialog\" role=\"document\">\r\n            <div class=\"modal-content\">\r\n                <div class=\"modal-header\">\r\n                    <h5 class=\"modal-title\" id=\"brokenLinkModal\">Broken Link!!!</h5>\r\n                </div>\r\n                <div class=\"modal-body\">\r\n                    <p>We are sorry, the link that you are accessing is now expired, Should you wish to continue please\r\n                        click the 'Request' button below to create a new link or 'Close' to exit.</p>\r\n                </div>\r\n                <div class=\"modal-footer\">\r\n                    <button type=\"button\" class=\"btn btn-danger\" data-bs-dismiss=\"modal\">Close</button>\r\n                    <button type=\"button\" class=\"btn btn-success\" id=\" requestNewLink\" title=\"requestNewLink\"\r\n                        data-bs-dismiss=\"modal\"\r\n                        onclick=\"requestNewLink('2EB493CE-C1B2-41C6-ADD7-7659EEA88E2A')\">Request</button>\r\n                </div>\r\n\r\n\r\n            </div>\r\n        </div>\r\n    </div>\r\n";
            return base.Content(httpResp, "text/html");
            

        }


    }
}
