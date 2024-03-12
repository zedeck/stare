using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using mie.era.mvc.Helpers;
using mie.era.mvc.Models;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol.Plugins;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
//using static BackendAPI.Services.RefereesService;    //To Change this

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class RefereesController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _config;

    public RefereesController(IHttpClientFactory clientFactory, IConfiguration config)
    {
        _clientFactory = clientFactory;
        _config = config;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? pageIndex)
    {
        int pageSize = 30;
        int pageNumber = pageIndex ?? 1;

        TempData["PageNumber"] = pageIndex;

        var client = _clientFactory.CreateClient();
       
        client.BaseAddress = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
        client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage refereesResponse = await client.GetAsync("Referees/GetRefereesWithCompletedReferences");

        if (refereesResponse.IsSuccessStatusCode)
        {
            var refereeces = await refereesResponse.Content.ReadAsAsync<List<RefereeInfo>>();

            int totalItems = refereeces.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedReferees = refereeces.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            List<Task<(DateTime, int)>> taskList = new List<Task<(DateTime, int)>>();

            foreach (var referee in paginatedReferees)
            {
                Task<(DateTime, int)> task = GetRefereeNotificationInfo(client, (int)referee.RequestID);
                taskList.Add(task);
            }

            await Task.WhenAll(taskList);

            var refereeNotifications = taskList.Select(t => t.Result).ToList();

            ReferenceViewModel referenceViewModel = new ReferenceViewModel();
            referenceViewModel.refereeces = paginatedReferees;
            referenceViewModel.AverageCandidateScore = 65; // refereeces.Average(r => r. ?? 0);
            referenceViewModel.AverageCompletionTime = refereeces.Count;
            referenceViewModel.CompletedReferencesCount = refereeces.Count(r => r.Status == "Completed");
            referenceViewModel.TotalReferencesCount = refereeces.Count;
            referenceViewModel.RefereeNotifications = refereeNotifications;
            referenceViewModel.lastReminderTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            referenceViewModel.PageIndex = pageNumber;
            referenceViewModel.TotalPages = totalPages;
            referenceViewModel.SearchText = "";
            return View(referenceViewModel);
        }
        else
        {
            ErrorViewModel models = new ErrorViewModel();
            return View("Error", models);
        }
    }
    


    private async Task<(DateTime, int)> GetRefereeNotificationInfo(HttpClient client, int refereeId)
    {
        try
        {
            client.DefaultRequestHeaders.Remove("MIEAuthorization");
            client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            HttpResponseMessage maxNotificationResponse = await client.GetAsync($"Statistics/GetMaxNotificationDateTime?refereeId={refereeId}");
            HttpResponseMessage notificationCountResponse = await client.GetAsync($"Statistics/GetNotificationCount?refereeId={refereeId}");

            if (maxNotificationResponse.IsSuccessStatusCode && notificationCountResponse.IsSuccessStatusCode)
            {
                DateTime maxNotificationDateTime = await maxNotificationResponse.Content.ReadAsAsync<DateTime>();
                int notificationCount = await notificationCountResponse.Content.ReadAsAsync<int>();

                return (maxNotificationDateTime, notificationCount);
            }
            else
            {
                // Handle error or return default values
                return (DateTime.MinValue, 0);
            }
        }
        catch (Exception ex)
        {
            // Handle exception or return default values
            return (DateTime.MinValue, 0);
        }
    }

    [HttpGet]
    //[Route("api/Referees/GetRefereesWithCompletedReferences")]
    public async Task<IActionResult> GetRefereesWithCompletedReferences()
    {
        try
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
            client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));

            HttpResponseMessage response = await client.GetAsync("Referees/GetRefereesWithCompletedReferences");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return Content(responseData, "text/plain");
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }

    [HttpGet]
    [Route("Statistics/GetMaxNotificationDateTime")]
    public async Task<IActionResult> GetMaxNotificationDateTime(int refereeId)
    {
        try
        {
            var client = _clientFactory.CreateClient();
           
            client.BaseAddress = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            HttpResponseMessage response = await client.GetAsync($"Statistics/GetMaxNotificationDateTime?refereeId={refereeId}");

            if (response.IsSuccessStatusCode)
            {
                DateTime maxNotificationDateTime = await response.Content.ReadAsAsync<DateTime>();
                return Ok(maxNotificationDateTime);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }


    [HttpGet]
    [Route("Statistics/GetNotificationCount")]
    public async Task<IActionResult> GetNotificationCount(int refereeId)
    {
        try
        {
            var client = _clientFactory.CreateClient();
           
            client.BaseAddress = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            HttpResponseMessage response = await client.GetAsync($"Statistics/GetNotificationCount?refereeId={refereeId}");
        }
        catch (Exception ex) { }

        return View();
    }



    [HttpGet("{id}/{page}")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, int page)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
        client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        Console.WriteLine("PageNumber is-----------------------------------------------------------: " + page);
     
        var reqStatus = client.GetAsync($"Requests/GetRequestStatus?RequestKey={id}").Result;
        if (reqStatus.IsSuccessStatusCode)
        {

            string resp = reqStatus.Content.ReadAsStringAsync().Result;
            if(resp.Equals("\"Reference\""))
            {
                var answeredForm = client.GetAsync($"Answers/GetReferenceAnswers/{id}").Result;
                if (answeredForm.IsSuccessStatusCode)
                {

                    var resp1 = await answeredForm.Content.ReadAsStringAsync();
                    ViewBag.Message = resp1;
                    return View();
                }
                else
                {
                    return View();
                     
                }
            }
            else
            {
                if (resp.Equals("\"Pending\""))
                {
                    //return NotFound();
                    var unansweredForm = client.GetAsync($"Questionnaire/GetAManuallyFillForm/requestKey?requestKey={id}").Result;
                    if (unansweredForm.IsSuccessStatusCode)
                    {

                        var resp2 = await unansweredForm.Content.ReadAsStringAsync();

                        ViewBag.Message = resp2;
                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }

            }

        }
        else
        {
            return View();
        }



        

        
      
    }

    // POST: Referees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Referee referee)
    {
        if (ModelState.IsValid)
        {
            var client = _clientFactory.CreateClient();
            
            client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
            client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PutAsJsonAsync($"Referees/{id}", referee);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        return View(referee);
    }

    [HttpGet()]
    public async Task<IActionResult> SearchReference(string? id)
    {
        try
        {
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Referees");

            int pageSize = 30;
            int pageNumber = 1;

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
            client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));

            HttpResponseMessage response = await client.GetAsync("Referees/SearchReference?filterInput=" + id);

            if (response.IsSuccessStatusCode)
            {
                var references = await response.Content.ReadAsAsync<List<RefereeInfo>>();

                int totalItems = references.Count;
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                //var paginatedReferees = refereeces.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


                ReferenceViewModel referenceViewModel = new ReferenceViewModel();
                referenceViewModel.refereeces = references;
                referenceViewModel.AverageCandidateScore = 65; // refereeces.Average(r => r. ?? 0);
                referenceViewModel.AverageCompletionTime = references.Count;
                referenceViewModel.CompletedReferencesCount = references.Count(r => r.Status == "Completed");
                referenceViewModel.TotalReferencesCount = references.Count;
//                referenceViewModel.RefereeNotifications = refereeNotifications;
                referenceViewModel.lastReminderTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                referenceViewModel.PageIndex = pageNumber;
                referenceViewModel.TotalPages = totalPages;
                referenceViewModel.SearchText = id;
                return View("Index", referenceViewModel);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------------
    [HttpGet("{id}/{index}")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> EditForm(string id, int index)
    {

        List<LRefereeEditQAModel> qaResponseData = new List<LRefereeEditQAModel>();
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
        client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));



        string referenceRenewUrl = client.BaseAddress.ToString() + "Questionnaire/RenewReference/RequestKey?RequestKey=" + id;
        ViewBag.ReferenceRenewUrl = referenceRenewUrl;
        string postEditedAnswersUrl = client.BaseAddress.ToString() + "Answers/PostEditedAnswers";
        ViewBag.PostEditedAnswersUrl = postEditedAnswersUrl;

        var reqStatus = client.GetAsync($"Requests/GetRequestStatus?RequestKey={id}").Result;
        if (reqStatus.IsSuccessStatusCode)
        {

            string resp = reqStatus.Content.ReadAsStringAsync().Result;
            if (resp.Equals("\"Reference\""))
            {
                HttpResponseMessage answeredForm  = client.GetAsync($"Answers/GetExistingReferenceForEdit/id?id={id}").Result;
                
                if (answeredForm.IsSuccessStatusCode)
                {
                    string qaResponseString = answeredForm.Content.ReadAsStringAsync().Result;
                    var qaFromDB = JsonConvert.DeserializeObject<List<LRefereeEditQAModel>>(qaResponseString)!;
                    int questionSetId = 0;

                    foreach (var qa in qaFromDB)
                    {
                        questionSetId = qa.QuestionnaireSetId;

                        qaResponseData.Add(
                                new LRefereeEditQAModel
                                {
                                    Answer = qa.Answer,
                                    AnswerDate = qa.AnswerDate,
                                    AnswerType = qa.AnswerType,
                                    AnswerWeight = qa.AnswerWeight,
                                    CandidateEmail = qa.CandidateEmail,
                                    CandidateFirstName = qa.CandidateFirstName,
                                    CandidateLastName = qa.CandidateLastName,
                                    CandidateName = qa.CandidateName,
                                    Id = qa.Id,
                                    Question = qa.Question,
                                    QuestionDate = qa.QuestionDate, 
                                    QuestionID = qa.QuestionID,
                                    QuestionnaireSetId = qa.QuestionnaireSetId,
                                    QuestionnaireSetName = qa.QuestionnaireSetName,
                                    RefereeCompanyName = qa.RefereeCompanyName,
                                    RefereeCountry = qa.RefereeCountry,
                                    RefereeEmail = qa.RefereeEmail,
                                    RefereeFirstName = qa.RefereeFirstName,
                                    RefereeID = qa.RefereeID,
                                    RefereeJobTitle = qa.RefereeJobTitle,
                                    RefereeLastName = qa.RefereeLastName,
                                    RefereeMobile = qa.RefereeMobile,
                                    RefereeName = qa.RefereeName,
                                    RefereeSpeciality = qa.RefereeSpeciality,
                                    ReferenceType = qa.ReferenceType,
                                    RequestID = qa.RequestID,
                                    RequestKey = qa.RequestKey
                                    

                                }
                            );


                    }

                    ViewBag.Message = "Filed";
                    ViewBag.Reference = id;
                    ViewBag.Model = qaResponseData;
                    
                   
                    
                    List<AnswerOptionsModel> _options = new List<AnswerOptionsModel>();  
                    HttpResponseMessage answerOptions = client.GetAsync($"Answers/GetAnswerOptions/id?questionSetId={questionSetId}").Result;
                    if (answerOptions.IsSuccessStatusCode)
                    {
                        string aOptions = answerOptions.Content.ReadAsStringAsync().Result;
                        var aOption = JsonConvert.DeserializeObject<List<AnswerOptionsModel>>(aOptions)!;
                        foreach(var aOpt in aOption)
                        {
                            _options.Add(new AnswerOptionsModel
                            {
                               QuestionID = aOpt.QuestionID,
                               AnswerOptions = aOpt.AnswerOptions

                            });
                        }
                    }
                        ViewBag.AnswerOptions = _options;
                        return View();
                }
                else
                {
                    ViewBag.Message = "NoFilledFormFound";
                    return View();
                }
            }
            else
            {
                if (resp.Equals("\"Pending\""))
                {
                    
                    var unansweredForm = client.GetAsync($"Questionnaire/GetAManuallyFillForm/requestKey?requestKey={id}").Result;
                    //var unansweredForm = client.GetAsync($"Answers/GetUnfilledReference/id?id={id}").Result;    --- to switch to this one
                    if (unansweredForm.IsSuccessStatusCode)
                    {

                        var resp2 = await unansweredForm.Content.ReadAsStringAsync();
                        ViewBag.Message = "NotFiled";
                        ViewBag.Form = resp2;
                        ViewBag.Reference = id;
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "NoUnFilledFormFound";
                        return View();
                    }
                }
                else
                {
                    if(resp.Equals("\"Expired\""))
                    {
                        ViewBag.Message = "Expired";
                        return View();

                    }
                    else
                    {
                        ViewBag.Message = "NoRecordFound";
                        return View();
                    }
                }


            }

        }
        else
        {
            ViewBag.Message = "SystemError";
            return View();
        }
    }

    [HttpGet]
    public PartialViewResult EditReferee(int requestId)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
        client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));

        HttpResponseMessage response = client.GetAsync("Referees/GetReferee?requestId=" + requestId).Result;

        if (response.IsSuccessStatusCode)
        {
            var referee = response.Content.ReadAsAsync<RefereeEdit>().Result;
            return new PartialViewResult
            {
                ViewName = "RefereeEdit",
                ViewData = new ViewDataDictionary<RefereeEdit>(ViewData, referee)
            };
        } else
        {
            throw new Exception("Error load ing referee");
        }

        //    RefereeEdit edt = new RefereeEdit();
        //edt.RefereeId = 1;
        //edt.RelationShip = "Boeta";
        //edt.PhoneNumber = "0836450654";
        //edt.Email = "jeandrep@gmail.com";
        //edt.Name = "jeandre";
        //edt.IsActive = true;
        
        ////return Partial("_ProductDetails", await productService.GetProductAsync(id));
        ////return PartialView("RefereeEdit", edt);
        //return new PartialViewResult
        //{
        //    ViewName = "RefereeEdit",
        //    ViewData = new ViewDataDictionary<RefereeEdit>(ViewData, edt)
        //};
    }

    [HttpPost]
    public async Task<IActionResult> SaveReferee([FromForm] RefereeEdit refereeData)
    {
        HttpClientHandler clientHandler = new()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            Credentials = CredentialCache.DefaultCredentials,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };

        var _httpClient = new HttpClient(clientHandler);

        Uri baseUrl = new Uri(_config.GetConnectionString("ERABackendEndpoint"));
        _httpClient.BaseAddress = baseUrl;
        _httpClient.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

        var json = System.Text.Json.JsonSerializer.Serialize(refereeData);
        var datax = new StringContent(json, Encoding.UTF8, "application/json");
        //-----
        HttpResponseMessage createBackendReq = _httpClient.PostAsync(_httpClient.BaseAddress + "Referees/SaveReferee", datax).Result;
        createBackendReq.EnsureSuccessStatusCode();
        if (createBackendReq.IsSuccessStatusCode)
        {
            int pageNumber = 1;
            if (TempData["PageNumber"] != null)
                pageNumber = Convert.ToInt32(TempData["PageNumber"]);
            //string data = createBackendReq.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index", "Referees", new { pageIndex = pageNumber });
        }
        else
        {
            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<IActionResult> CancelRequest(int id)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new System.Uri(_config.GetConnectionString("ERABackendEndpoint").ToString());
        client.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + HttpContext.GetToken());

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));

        HttpResponseMessage response = client.GetAsync("Requests/CancelRequest?requestId=" + id).Result;

        if (response.IsSuccessStatusCode)
        {
            int pageNumber = 1;
            if (TempData["PageNumber"] != null)
                pageNumber = Convert.ToInt32(TempData["PageNumber"]);
            //string data = createBackendReq.Content.ReadAsStringAsync().Result;
            return RedirectToAction("Index", "Referees", new { pageIndex = pageNumber });
        }
        else
        {
            return StatusCode(500);
        }

    }
}
