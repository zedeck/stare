using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using mie.era.mvc.Helpers;
using mie.era.mvc.Models;
using Newtonsoft.Json;
using System.Net;

namespace mie.era.mvc.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CandidatesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;


        public CandidatesController(IConfiguration config)
        {
            _config = config;

            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            
            _httpClient = new HttpClient(clientHandler);
        }

        [HttpGet]
        public IActionResult Index()
        {
            #region: hide
            //Date Picker
            string todaysDate = DateTime.Now.ToString("yyyyy-MM-dd");
            var mydate = new DateViewModel() { DateStart = todaysDate };



            List<FELCandidateListViewModel> listOfCandidates = new List<FELCandidateListViewModel>();
            int numberOfCandidates = 0;
            int totalNumberOrReferences = 0;
            int numberOfcompletedReferences = 0;

            Uri baseUrl = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
            _httpClient.BaseAddress = baseUrl;
            _httpClient.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            HttpResponseMessage candidateCount = _httpClient.GetAsync(_httpClient.BaseAddress + "Candidate/GetCandidateCount").Result;

            if (candidateCount.IsSuccessStatusCode)
            {
                string data = candidateCount.Content.ReadAsStringAsync().Result;
                numberOfCandidates = int.Parse(data);

            }

            HttpResponseMessage completedReferences = _httpClient.GetAsync(_httpClient
            .BaseAddress + "Candidate/GetNumberOfCompletedReferences").Result;

            if (completedReferences.IsSuccessStatusCode)
            {
                string data = completedReferences.Content.ReadAsStringAsync().Result;
                numberOfcompletedReferences = int.Parse(data);

            }


            HttpResponseMessage totalReferences = _httpClient.GetAsync(_httpClient
            .BaseAddress + "Candidate/GetTotalNumberOfReferences").Result;

            if (totalReferences.IsSuccessStatusCode)
            {
                string data = totalReferences.Content.ReadAsStringAsync().Result;
                totalNumberOrReferences = int.Parse(data);

            }


            HttpResponseMessage response = _httpClient.GetAsync(_httpClient
                .BaseAddress + "Candidate/GetListOfCandidates").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                listOfCandidates = JsonConvert.DeserializeObject<List<FELCandidateListViewModel>>(data)!;    


            }

            ViewBag.Message = "Candidates View";
            ViewBag.NumberOfCandidates = numberOfCandidates;
            ViewBag.TotalReferences = totalNumberOrReferences;
            ViewBag.CompletedReferences = numberOfcompletedReferences;
            ViewBag.MyDate = mydate;


            ViewBag.model = listOfCandidates;
            #endregion
            return View();
        }

        [HttpGet("RequestID")]
        public ActionResult Details(int? RequestID)
        {

            Uri baseUrl = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
            _httpClient.BaseAddress = baseUrl;

            if (RequestID == null)
            {
                return BadRequest();
            }

            FELCandidateViewModel candidateDetails = new FELCandidateViewModel();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient
           .BaseAddress + "Candidate/GetCandidateInfoByReqID?RequestID=" + RequestID).Result;
            _httpClient.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            if ( response == null)
            {
                return BadRequest();
            }

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                 candidateDetails = JsonConvert.DeserializeObject<FELCandidateViewModel>(data)!;    


            }
            return View(candidateDetails);
        }


        [HttpGet("RequestID")]
        public ActionResult Edit(int? RequestID)
        {

            Uri baseUrl = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
            _httpClient.BaseAddress = baseUrl;
            _httpClient.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            if (RequestID == null)
            {
                return BadRequest();
            }

          
            CandidateRazorModel candidateRazorModel = new CandidateRazorModel();
           

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient
           .BaseAddress + "Candidate/EditCandidateInfo?RequestID=" + RequestID).Result;

            if (response == null)
            {
                return BadRequest();
            }

            if (response.IsSuccessStatusCode)
            {
                candidateRazorModel.DataContent = response.Content.ReadAsStringAsync().Result;
              
            }
            return View(candidateRazorModel);
        }
    }
}
