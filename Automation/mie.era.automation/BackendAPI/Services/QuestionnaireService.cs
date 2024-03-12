using BackendAPI.Constants;
using BackendAPI.Database;
using BackendAPI.Global;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace BackendAPI.Services
{
    public class QuestionnaireService : IQuestionnaire
    {

        private readonly IDatabaseRepo _dbrepo;
        private readonly IDashboard _dashsrv;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICandidate _candisrv;
        private readonly CommsService _commsSrv;
        private readonly HttpClient _httpClient;

        public QuestionnaireService(IDatabaseRepo dbrepo,  IDashboard dashsrv, IConfiguration configuration, IHttpClientFactory httpClientFactory, ICandidate candisrv, CommsService commsSrv)
        {
            _dbrepo = dbrepo;
            _dashsrv = dashsrv;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _candisrv = candisrv;
            _commsSrv = commsSrv;
           
            

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
            _httpClient = new HttpClient(handler);
        }

              

        #region: Html Questions
        public List<LQuestionsModel> GetQuestionsFormBySetID(LCreateFormRequestModel LCFRequestModel)
        {

            GlobalVars.inlineScript = "";
            GlobalVars.inlineStyle = "";

            //PCVNumber
             
            string[] extractPCVNumber = LCFRequestModel.CandidateID.Split('-');
            int pcvNumber = 0;
            if(extractPCVNumber.Length > 1 )
                pcvNumber = int.Parse(extractPCVNumber[0]);

            string requestKey = LCFRequestModel.CandidateID; 

            string dateConducted = DateTime.Now.ToString();
            string noConsentURL = _configuration.GetConnectionString("NoConsentURL").ToString();
            string frontEndControlType;

            int questionSetID = LCFRequestModel.QuestionSetID;
            string questionID;
            string questionText;
            string answerGroupID;
            int secondQuestionID;
            int answerWeight = 0;
            Guid referenceUUID = Guid.NewGuid();

            //Form  data urls
            string formPostReturnEndPoint = _configuration.GetConnectionString("PostAnswersEndPoint").ToString();


            //Candidate
            string candidateID = LCFRequestModel.CandidateID;
            string candidateName = string.Empty;
            string candidateFirstName = string.Empty;
            string candidateLastName = string.Empty;
            string candidateEmail = string.Empty;
            string candidatePhone = string.Empty;

            //Get Candidate Details
            LCandidateModel getCandidateDetails = _candisrv.GetCandidateInfo(candidateID);
            if (getCandidateDetails != null)
            {
               
                candidateName = getCandidateDetails.CandidateName;
                string[] candidateNameSplit = candidateName.Split(' ');
                if(candidateNameSplit.Length > 1 )   //If theres more than one name take the first
                {
                    candidateFirstName = candidateNameSplit[0];
                }
                else
                {
                    candidateFirstName = getCandidateDetails.CandidateName;
                }
                candidateLastName = getCandidateDetails.CandidateSurname;
                candidateEmail = getCandidateDetails.CandidateEmail;
                candidatePhone = getCandidateDetails.CandidateCell!;    
            }

            //Get Referee Information
            int refereeID = LCFRequestModel.RefereeID;
            string refereeName = string.Empty;  
            string refereeFirstName = string.Empty;
            string refereeLastName = string.Empty;
            string refereeEmail = string.Empty;
            string refereeCountry = string.Empty;
            string refereeMobileNumber = string.Empty;
            string refereeCompanyName = string.Empty;
            string referenceType = string.Empty;
            string refereeJobTitle = string.Empty;
            string refereeSpeciality = string.Empty;
            string? refereeRelationship = string.Empty;


            //Get Referee Details
            var refereeData = _dbrepo.GetRefereeInfo(refereeID);
            if (refereeData != null)
            {
                ReferenceExtendedData extData = _dbrepo.GetReferenceExtendedData(LCFRequestModel.RequestNumber);

                refereeName = refereeData.Name!;
                string[] refereeNameAndSurname = refereeName.Split(' ');
                refereeFirstName = refereeNameAndSurname[0];
                if (refereeNameAndSurname.Length > 1)
                    refereeLastName = refereeNameAndSurname[1];
                else
                    refereeLastName = string.Empty;

                refereeEmail = refereeData.Email!;
                refereeCountry = extData.Country;//string.Empty;
                refereeMobileNumber = refereeData.PhoneNumber!.ToString();
                refereeCompanyName = extData.CompanyName;// string.Empty;
                referenceType = extData.ReferenceType;// string.Empty;
                refereeJobTitle = extData.JobTitle;// string.Empty;
                refereeSpeciality = string.Empty;
                refereeRelationship = string.Empty;
            }

                  
            //Get referee from databasee data in here
            //---------------------------------------
            LRequestModel lRequestModel = new()
            {
                refereeEmail = refereeEmail,
                refereeName = refereeName,
                refereePhoneNumber = refereeMobileNumber,
                questionSetId = questionSetID,
                relationsShip = refereeRelationship,
                remoteKey = candidateID.ToString(),
                refereeId = refereeID


            };

            //Create Request
            //---------------------------------------
            //int retquestID = CreateRequest(lRequestModel);

            int retquestID = LCFRequestModel.RequestNumber;   //pcv number is the request ID

            List<LQuestionsModel> result = new();

            Task<IEnumerable<Question>> response = _dbrepo.GetQuestionsBySetID(LCFRequestModel.QuestionSetID);
            
            string json = JsonConvert.SerializeObject(response.Result, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            if (!json.Any())
            {
                result.Clear();
                return result;
            }

            var jsonObj = JsonConvert.DeserializeObject<dynamic>(json);
            string header = "<!doctype html>\r\n<html lang=\"en\">\r\n  <head>\r\n    <meta charset=\"utf-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Mie-Reference</title>\r\n "+
                "<script src=\"https://code.jquery.com/jquery-3.7.1.js\"\r\n></script> \r\n" +
                "<script src =\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-C6RzsynM9kWDrMNeT87bh95OGNyZPhcTNXj1NW7RuBCsyN/o0jlpcV8Qyq46cDfL\" crossorigin=\"anonymous\"></script>\r\n" +
                "<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN\" crossorigin=\"anonymous\">\r\n" +
                " <script src=\"https://cdn.jsdelivr.net/npm/sweetalert2@7.12.15/dist/sweetalert2.all.min.js\"></script>\r\n  " +
                "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@7.12.15/dist/sweetalert2.min.css'>\r\n  </link>" +
                "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/js/bootstrap-multiselect.js\"></script>\r\n  <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/css/bootstrap-multiselect.css\">"+
                "<style>" +
                ".rating {\r\n      display: flex;\r\n      flex-direction: row-reverse;\r\n      justify-content: start;\r\n      gap: 25px;\r\n\r\n    }\r\n\r\n    .rating>input {\r\n      display: none\r\n    }\r\n\r\n    .rating>label {\r\n      position: relative;\r\n      width: 1em;\r\n      font-size: 30px;\r\n      font-weight: 300;\r\n      color: #FFD600;\r\n      cursor: pointer\r\n    }\r\n\r\n    .rating>label::before {\r\n      content: \"\\2605\";\r\n      position: absolute;\r\n      opacity: 0\r\n    }\r\n\r\n    .rating>label:hover:before,\r\n    .rating>label:hover~label:before {\r\n      opacity: 1 !important\r\n    }\r\n\r\n    .rating>input:checked~label:before {\r\n      opacity: 1\r\n    }\r\n\r\n    .rating:hover>input:checked~label:before {\r\n      opacity: 0.4\r\n    }" +
                "INLINE_STYLE</style>" +
                            "\r\n</head>";
            string candidateMsgInfo = StringConstants.REF_FORM_HEADER2.ToString();
            string newCandidateMsgInfo = candidateMsgInfo.Replace("CandidateName",candidateName);
            string noConsentMsgInfo = StringConstants.REF_FORM_NOCONSENT.ToString();
            string fullNoConsentEndpoint = _configuration.GetConnectionString("NoConsetEndpoint").ToString();
            string singleQuotedStr = "'" + fullNoConsentEndpoint +referenceUUID+"'";
            noConsentURL = noConsentURL.Replace("NC_ARG",singleQuotedStr);
            string newNoCosentMsgInfo = noConsentMsgInfo.Replace("NoConsentReturnPage", noConsentURL);

            string answerTypeForm = header + "\r\n<body>\r\n" +
                " <div class=\"row my-8 justify-content-center\">\r\n\t\t<div class=\"col-md-8\">\r\n\t\t<a class=\"navbar-brand\" href=\"#\">\r\n\t\t\t\t<img src=\"https://qa.mie.co.za/Internal/Apps/epcv/images/mie2.png\" width=\"80\" height=\"60\" alt=\"\">\r\n\t\t</a>\r\n\t\t<br>\r\n\t\t</div>\r\n\t\t<br>\r\n\t    \r\n</div>" +
                " <div class=\"row my-8 justify-content-center bg-secondary text-dark bg-opacity-10\">\r\n <div class=\"col-md-8\">\r\n        <br>\r\n\t\t<h5 class=\"fw-bold\" id=\"RefereeName\">Hello " + refereeName + " </h5>\r\n\t\t<br>" + StringConstants.REF_FORM_HEADER1 + newCandidateMsgInfo +
                StringConstants.REF_FORM_DISCLAIMER + newNoCosentMsgInfo +
                "<div class=\"row my-8 justify-content-center\">\r\n\t\t<div class=\"col-md-6\">\r\n            <div class=\"form-floating\">\r\n\t\t\t \r\n                <span type=\"text\" id=\"PcvNumber\" name=\"PcvNumberControl\">MIE Refence Number(PCV): <b>" + requestKey + "</b><span>\r\n            </div>\r\n        </div>\r\n \r\n        <div class=\"col-md-6\">\r\n            <div class=\"form-floating text-end\">\r\n\t\t\t  <span type=\"datetime\" id=\"DateConducted\" name=\"DateConductedCotrol\" >Date Conducted: <b>" + dateConducted + "</b><span>\r\n\t\t    </div>\r\n        </div>\r\n\t\t</div>\r\n<hr>" +

                "<form  method=\"POST\"\r\n  class=\"w-100 rounded-1 p-4 border bg-secondary text-dark bg-opacity-10 needs-validation\" \r\n  action=\""+ formPostReturnEndPoint + "\"  enctype=\"multipart/form-data\" novalidate>\r\n" +
                "<h5 class=\"fw-bold\">Please verify your details below:</h5>\r\n\t<br>\r\n"+
                    "<div class=\"row my-8\"> \r\n"+
                        "<div class=\"col-md-6\">\r\n"+
                            "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                "<input type=\"text\" id=\"RefereeFirstNameControl\" name=\"RefereeFirstName\" class=\"form-control\" value=\"" + refereeFirstName + "\" required/>\r\n" +
                                "<label class=\"form-label\" for=\"RefereeFirstNameControl\">First name</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid first name.\r\n</div>" +
                            "</div>\r\n"+
                        "</div>\r\n"+
                        "<div class=\"col-md-6\">\r\n"+
                            "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                "<input type=\"text\" id=\"RefereeLastNameControl\" name=\"RefereeLastName\" class=\"form-control\" value=\"" + refereeLastName + "\" required/>\r\n" +
                                "<label class=\"form-label\" for=\"RefereeLastNameControl\">Last name</label>\r\n" +
                                 "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid last name.\r\n</div>" +
                            "</div>\r\n" +
                        "</div>\r\n"+
                    "</div>\r\n\t<br>\r\n\t"+
                        "<div class=\"row my-8\"> \r\n"+
                            "<div class=\"col-md-6\">\r\n"+
                                "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                    "<input type=\"email\" id=\"RefereeEmailControl\" name=\"RefereeEmail\" class=\"form-control\" value=\"" + refereeEmail+ "\" required/>\r\n" +
                                    "<label class=\"form-label\" for=\"RefereeEmailControl\">Email</label>\r\n" +
                                     "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid email.\r\n</div>" +
                                "</div>\r\n" +
                            "</div>\r\n"+
                            "<div class=\"col-md-2\">\r\n"+
                                "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                    "<input type=\"tel\" readonly id=\"RefereeCountryControl\" name=\"RefereeCountry\" class=\"form-control\"  value=\"" + refereeCountry+ "\" required/>\r\n" +
                                    "<label class=\"form-label\" for=\"RefereeCountryControl\">Country</label>\r\n" +
                                     "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid country name.\r\n</div>" +
                                "</div>\r\n" +
                            "</div>\r\n\t\t"+
                            "<div class=\"col-md-4\">\r\n"+
                                "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                    "<input type=\"text\" id=\"RefereeMobileNumberControl\" name=\"RefereeMobileNumber\" class=\"form-control\" value=\"" + refereeMobileNumber+ "\" required/>\r\n" +
                                    "<label class=\"input-group-addon\" for=\"RefereeMobileNumberControl\">Mobile number</label>\r\n" +
                                     "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid mobile number.\r\n</div>" +
                                "</div>\r\n" +
                            "</div>\r\n"+
                        "</div>\r\n\t<br>\r\n\t"+
                            "<div class=\"row my-8\"> \r\n"+
                                "<div class=\"col-md-3\">\r\n"+
                                    "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                        "<input type=\"text\" readonly id=\"RefereeCompanyNameControl\" name=\"RefereeCompanyName\" class=\"form-control\" value=\"" + refereeCompanyName+ "\" required/>\r\n" +
                                        "<label class=\"form-label\" for=\"RefereeCompanyNameControl\">Company name</label>\r\n" +
                                         "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid company name.\r\n</div>" +
                                    "</div>\r\n" +
                                "</div>\r\n"+
                                "<div class=\"col-md-3\">\r\n"+
                                    "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                       "<input type=\"text\" readonly id=\"ReferenceTypeControl\" name=\"ReferenceType\" class=\"form-control\" value=\"" + referenceType+ "\"/ required>\r\n" +
                                       "<label class=\"form-label\" for=\"ReferenceTypeControl\">Reference type</label>\r\n" +
                                        "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid reference type.\r\n</div>" +
                                    "</div>\r\n" +
                                "</div>\r\n\t\t"+
                                "<div class=\"col-md-3\">\r\n"+
                                    "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                        "<input type=\"text\" readonly id=\"RefereeJobTitleControl\" name=\"RefereeJobTitle\" class=\"form-control\" value=\"" + refereeJobTitle+ "\" required/>\r\n" +
                                        "<label class=\"form-label\" for=\"RefereeJobTitleControl\">Referee job title</label>\r\n" +
                                         "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid job title.\r\n</div>" +
                                    "</div>\r\n" +
                                "</div>\r\n\t\t"+
                                "<div class=\"col-md-3\">\r\n"+
                                    "<div class=\"form-floating\">\r\n\t\t\t \r\n"+
                                        "<input type=\"text\" id=\"RefereeSpecialityControl\" name=\"RefereeSpeciality\" class=\"form-control\" value=\"" + refereeSpeciality+ "\"/>\r\n" +
                                        "<label class=\"form-label\" for=\"RefereeSpecialityControl\">Speciality</label>\r\n" +
                                    "</div>\r\n"+
                                "</div>\r\n"+
                                "</div>\r\n\t"+
                                "<br>\r\n\t"+
                                "<br>\r\n\t\r\n\t\r\n"+
                                "<hr class=\"my-1\">\t\r\n"+
                                "<br>\r\n"+
                                "<h5 class=\"fw-bold\">Reference Questions</h5>\r\n<br>\r\n" +
                                "<div style=\"display: none;\">"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"QuestionSetControl\" name=\"QuestionSetID\" value=\"" + questionSetID + "\" ><br> \r\n" +
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"RequestControl\" name=\"RequestKey\" value=\"" + requestKey + "\" ><br> \r\n" +
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"RequestID\" name=\"RequestID\" value=\"" + retquestID + "\" ><br> \r\n"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"RefereeName\" name=\"RefereeName\" value=\"" + refereeName + "\" ><br> \r\n" +
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"CandidateName\" name=\"CandidateName\" value=\"" + candidateName + "\" ><br> \r\n"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"CandidateFirstName\" name=\"CandidateFirstName\" value=\"" + candidateFirstName + "\" ><br> \r\n"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"CandidateLastName\" name=\"CandidateLastName\" value=\"" + candidateLastName + "\" ><br> \r\n"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"CandidateEmail\" name=\"CandidateEmail\" value=\"" + candidateEmail + "\" ><br> \r\n"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"RefereeID\" name=\"RefereeID\" value=\"" + refereeID + "\" ><br> \r\n"+
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"ReferenceUUID\" name=\"ReferenceUUID\" value=\"" + referenceUUID + "\" ><br> \r\n" +
                                "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"AnswerWeight\" name=\"AnswerWeight\" value=\"" + answerWeight + "\" ><br> \r\n </div> \r\n";


            if (jsonObj != null)
            {
                int QNumber = 0;  //Question Number.

                foreach (var jdata in jsonObj)
                {
                    QNumber++;

                    if (jdata.AnswerTypeId == null)
                    {
                        continue;
                    }
                    if (jdata.IsActive.ToString() == "True")   //Check if its an active Question
                    {


                        frontEndControlType = Convert.ToInt32(jdata.AnswerTypeId).ToString(); //_dbrepo.GetFrontEndControlTypeById(Convert.ToInt32(jdata.AnswerTypeId)).ToString();
                        questionID = jdata.QuestionId.ToString();
                        questionText = "Q" + QNumber + ". " + jdata.QuestionText.ToString();
                        answerGroupID = jdata.AnswerGroupId.ToString();

                        string secondQuestionText = _dbrepo.GetTrailingQText(Convert.ToInt16(jdata.QuestionId));
                        bool isLeadingQuestion;
                        string? leadingQuestionAnswer;
                        if (!String.IsNullOrEmpty(secondQuestionText))
                        {
                            secondQuestionID = _dbrepo.GetTrailingQID(secondQuestionText, Convert.ToInt16(jdata.QuestionId));
                            leadingQuestionAnswer = _dbrepo.GetTrailingQAnswer(secondQuestionID);
                            isLeadingQuestion = true;


                        }
                        else
                        {
                            isLeadingQuestion = false;
                            secondQuestionText = "";
                            leadingQuestionAnswer = "";
                        }

                        result.Add(new Models.LQuestionsModel { Question = questionText, AnswerType = GetHtmlForm(frontEndControlType, questionID, answerGroupID, isLeadingQuestion.ToString().ToLower(), secondQuestionText, leadingQuestionAnswer!), Id = jdata.QuestionId });
                        if (jdata.AnswerTypeId.ToString() == "3")  //If its YesNoOptions
                            answerTypeForm += "<span class=\"form-label d-block\" for=\"Control" + questionID + "\" >" + questionText + "</span> \r\n" + GetHtmlForm(frontEndControlType, questionID, answerGroupID, isLeadingQuestion.ToString().ToLower(), secondQuestionText, leadingQuestionAnswer!) + "\r\n<br>\r\n<hr class=\"my-1\"> \r\n<br>";
                        else
                            answerTypeForm += "<label class=\"d-block mb-4\" for=\"Control" + questionID + "\"> \r\n <span class=\"form-label d-block\">" + questionText + "</span> \r\n" + GetHtmlForm(frontEndControlType, questionID, answerGroupID, isLeadingQuestion.ToString().ToLower(), secondQuestionText, leadingQuestionAnswer!) + "</label> \r\n<br>\r\n <hr class=\"my-1\">\r\n <br>";

                    }

                }

                answerTypeForm += " <div class=\"mb-3\">\r\n " +
                    "<button type=\"submit\" id=\"submitForm\" class=\"btn btn-primary btn-lg px-3 rounded-3\">\r\n Send Answers\r\n </button>\r\n\r\n "+
                    "<button type=\"button\" id=\"saveForm\" class=\"btn btn-secondary btn-lg px-3 rounded-3 ms-3 disabled\">\r\n Continue Later\r\n </button>"+
                    " </div>\r\n</form> \r\n"+
                    "<script>\r\n " +
                    "(() => {\r\n  'use strict'\r\n\r\n  // Fetch all the forms we want to apply custom Bootstrap validation styles to\r\n  const forms = document.querySelectorAll('.needs-validation')\r\n\r\n  // Loop over them and prevent submission\r\n  Array.from(forms).forEach(form => {\r\n    form.addEventListener('submit', event => {\r\n      if (!form.checkValidity()) {\r\n        event.preventDefault()\r\n        event.stopPropagation()\r\n      }\r\n\r\n      form.classList.add('was-validated')\r\n    }, false)\r\n  })\r\n})();\r\n" +
                    GlobalVars.inlineScript + " \r\n   function noConsent(param) {\r\n const response = confirm(\"Are you sure you want to leave?\");\r\n if (response) {\r\n alert(\"We are Sorry for the inconvenience,we will alert our client.\");\r\n \r\n $.ajax({\r\n type: \"GET\",\r\n dataType: \"json\",\r\n url: param,\r\n success: function (data) {\r\n alert(data);\r\n },\r\n error: function (error) {\r\n\r\n jsonValue = jQuery.parseJSON(error.responseText);\r\n alert(\"error\" + error.responseText);\r\n }\r\n });\r\n\r\n \r\n window.location.href =\"http://qaweb01.miegalaxy.com/Internal/apps/era-mvc-int/api/Referees/Index\"; \r\n } else {\r\n alert(\"Thank you for your support, Please continue.\");\r\n }\r\n };" +
                   "$('#submitForm').click(function () {\r\n\r\n          const forms = document.querySelectorAll('.needs-validation')\r\n          Array.from(forms).forEach(form => {\r\n            form.addEventListener('submit', event => {\r\n              if (!form.checkValidity()) {\r\n                event.preventDefault()\r\n                event.stopPropagation()\r\n\r\n                $(form).find(\"[class^='form-']:invalid\").first().focus();\r\n\r\n              }\r\n\r\n              form.classList.add('was-validated')\r\n            }, false)\r\n\r\n          })\r\n\r\n          if ($('form')[0].checkValidity()) {\r\n\r\n            swal({\r\n              title: \"Thank You!\",\r\n              text: \"Your answers are submitted successfully!\",\r\n              type: \"success\"\r\n            }).then(function () {\r\n              window.location.href =\"http://qaweb01.miegalaxy.com/Internal/apps/era-mvc-int/api/Referees/Index\"; \r\n            });\r\n\r\n          }\r\n\r\n        });" +
                   "</script>\r\n  </div>\r\n</div>\r\n</div>\r\n</body>\r\n</html>";
            }
            else
            {
                result.Add(new Models.LQuestionsModel { Question = "empty", AnswerType = "empty", Id = 0 });
                answerTypeForm += "</form>"+
                    "<script>" +
                    "(() => {\r\n  'use strict'\r\n\r\n  // Fetch all the forms we want to apply custom Bootstrap validation styles to\r\n  const forms = document.querySelectorAll('.needs-validation')\r\n\r\n  // Loop over them and prevent submission\r\n  Array.from(forms).forEach(form => {\r\n    form.addEventListener('submit', event => {\r\n      if (!form.checkValidity()) {\r\n        event.preventDefault()\r\n        event.stopPropagation()\r\n      }\r\n\r\n      form.classList.add('was-validated')\r\n    }, false)\r\n  })\r\n})();\r\n" +
                     GlobalVars.inlineScript + " \r\n   function noConsent(param) {\r\n const response = confirm(\"Are you sure you want to leave?\");\r\n if (response) {\r\n alert(\"We are Sorry for the inconvenience,we will alert our client.\");\r\n \r\n $.ajax({\r\n type: \"GET\",\r\n dataType: \"json\",\r\n url: param,\r\n success: function (data) {\r\n alert(data);\r\n },\r\n error: function (error) {\r\n\r\n jsonValue = jQuery.parseJSON(error.responseText);\r\n alert(\"error\" + error.responseText);\r\n }\r\n });\r\n\r\n \r\n window.location.replace('https://mettus.co.za');\r\n } else {\r\n alert(\"Thank you for your support, Please continue.\");\r\n }\r\n };" +
                     "$('#submitForm').click(function () {\r\n\r\n          const forms = document.querySelectorAll('.needs-validation')\r\n          Array.from(forms).forEach(form => {\r\n            form.addEventListener('submit', event => {\r\n              if (!form.checkValidity()) {\r\n                event.preventDefault()\r\n                event.stopPropagation()\r\n\r\n                $(form).find(\"[class^='form-']:invalid\").first().focus();\r\n\r\n              }\r\n\r\n              form.classList.add('was-validated')\r\n            }, false)\r\n\r\n          })\r\n\r\n          if ($('form')[0].checkValidity()) {\r\n\r\n            swal({\r\n              title: \"Thank You!\",\r\n              text: \"Your answers are submitted successfully!\",\r\n              type: \"success\"\r\n            }).then(function () {\r\n            window.location.href =\"http://qaweb01.miegalaxy.com/Internal/apps/era-mvc-int/api/Referees/Index\"; \r\n            });\r\n\r\n          }\r\n\r\n        });" +
                     "</script>\r\n </div>\r\n</div>\r\n</div> \r\n</body>\r\n</html>";

            }

            answerTypeForm = answerTypeForm.Replace("INLINE_STYLE", GlobalVars.inlineStyle);

            System.IO.File.WriteAllText(@".\HtmlQuestionsForm.html", answerTypeForm);     //Do not forget to comment

            //Create a reference form.
            LReferenceForm lReferenceForm = new();

            byte[] formData = Encoding.ASCII.GetBytes(answerTypeForm);
            lReferenceForm.FormData = formData;
            lReferenceForm.UniqueId = referenceUUID; //Guid.NewGuid();
            
            Guid? SuniqueID = lReferenceForm.UniqueId;
            lReferenceForm.RequestId = retquestID; 
            lReferenceForm.Link = CreateReferenceLink(SuniqueID);
            lReferenceForm.CreationDate = DateTime.Now;

            DateTime dt = DateTime.Now;
            lReferenceForm.ExpiryDate = dt.AddDays(3);
            lReferenceForm.IsExpired = false;
            lReferenceForm.IsCompleted = false;
            lReferenceForm.IsActive = true;
            lReferenceForm.Status = 1;
            lReferenceForm.CreatedBy = "Godwill Makhubela";    //****Get Currect User in here

           
            if(String.IsNullOrEmpty(refereeEmail) && String.IsNullOrEmpty(refereeMobileNumber))
            {
                Console.WriteLine("Referee email or phonenumber does not exist");

            }
            else
            {
               

                string formLink = _dbrepo.CreateReferenceForm(lReferenceForm);
                LReminderModel commsData = new LReminderModel
                {
                    LinkURL = formLink,
                    RefereeEmail = refereeEmail,
                    RefereeName = refereeName,
                    RefereePhone =refereeMobileNumber,
                    ReferenceName = candidateName,
                    ReferenceType = 1,
                    SaveNotification = true,
                    RefereeId = refereeID,
                    

                };
                var rem = _commsSrv.SendComms(commsData);
                //Update Status
                 _dbrepo.RequestStatusUpdate(retquestID, "Awaiting Reference Response");

            }
         return result;
        }
        #endregion

        #region: Helper Methods
        public Task<List<AnswerType>> GetAllAnswerTypes()
        {
            return _dbrepo.GetAllAnswerTypes();
        }


        public string GetHtmlForm(string s_arg, string s_uniqueID, string s_answerGroupID, string s_IsLeadingQuestion, string s_LeadingQuestionText, string s_leadingQuestionAnswer)
        {
            string result;
            switch (s_arg)
            {
                //Text or string
                case "1":
                    result = "<input  class=\"form-control\" type=\"text\" id=\"Control" + s_uniqueID + "\" name=\"TextInput-" + s_uniqueID + "\" required > \r\n" +
                                "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid text input.\r\n</div>";
                    break;
                //Numeric Answer
                case "2":
                    result = "<input  class=\"form-control\" type=\"number\" id=\"Control" + s_uniqueID + "\" name=\"NumericInput-" + s_uniqueID + "\" min=\"1\" max=\"100\" required> \r\n" +
                        "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid numeric input.\r\n</div>";
                    break;
                //Yer or No
                case "3":
                    if (s_IsLeadingQuestion.ToLower() == "true")
                    {
                        result = "<fieldset>\r\n" +
                                   "<div>\r\n" +
                                       "<input class=\"form-check-input\" type=\"radio\" name=\"YesNo-" + s_uniqueID + "\" id=\"flexRadioDefault" + s_uniqueID + "No\" value=\"No\"  onclick=\"noradioFunction" + s_uniqueID + "()\"> \r\n" +
                                       "<label class=\"form-check-label\" for=\"flexRadioDefault" + s_uniqueID + "No\">\r\n No\r\n </label>\r\n" +
                                   "</div>\r\n" +
                                   "<div>\r\n" +
                                       "<input class=\"form-check-input\" type=\"radio\" name=\"YesNo-" + s_uniqueID + "\" id=\"flexRadioDefault" + s_uniqueID + "Yes\" value=\"Yes\" onclick=\"radioFunction" + s_uniqueID + "()\" required>\r\n" +
                                       "<label class=\"form-check-label\" for=\"flexRadioDefault" + s_uniqueID + "Yes\">\r\nYes\r\n </label>\r\n" +
                                       "<div class=\"invalid-feedback\">\r\n\tPlease select a valid option.\r\n</div>\r\n" +
                                   "</div>\r\n" +
                                   "<div id=\"flexInputDefault" + s_uniqueID + "\"> </div>\r\n"+
                        "</fieldset>";
                                 


                        if (!GlobalVars.inlineScript!.Contains("flexInputDefault" + s_uniqueID))
                        {
                            if (s_leadingQuestionAnswer.ToLower() == "yes")
                            {

                                GlobalVars.inlineScript += "function radioFunction" + s_uniqueID + "(){\r\n " +
                                                            "if(document.getElementById(\"textArea" + s_uniqueID + "\") ===null)\r\n{\r\n" +
                                                            "if(document.getElementById(\"flexInputDefault" + s_uniqueID + "\").innerText !== \"" + s_LeadingQuestionText + "\"){ \r\n" +
                                                            "document.getElementById(\"flexInputDefault" + s_uniqueID + "\").innerText = \"" + s_LeadingQuestionText + "\"; \r\n}\r\n" +
                                                            "const eli =  document.getElementById(\"flexInputDefault" + s_uniqueID + "\");\r\n" +
                                                            "const newline = document.createElement(\"br\");\r\n" +
                                                            "newline.id = \"newline1\";\r\n" +
                                                            "const newline2 = document.createElement(\"br\");\r\n" +
                                                            "newline2.id = \"newline2\";\r\n" +
                                                            "const textArea = document.createElement(\"textarea\");\r\n" +
                                                            "textArea.id = \"textArea" + s_uniqueID + "\";\r\n" +
                                                            "textArea.rows = \"2\";\r\n" +
                                                            "textArea.className = \"form-control\";\r\n" +
                                                            "textArea.name = \"TextInput-" + s_uniqueID + "\";\r\n" +
                                                            "eli.append(newline, newline2, textArea);\r\n" +
                                                            "}\r\n};\r\n" +

                                                            "function noradioFunction" + s_uniqueID + "(){\r\n " +
                                                            "if(document.getElementById(\"textArea" + s_uniqueID + "\") !==null)\r\n{\r\n" +
                                                            "const rmQuestion = document.getElementById(\"flexInputDefault" + s_uniqueID + "\");\r\n" +
                                                            "const remElement = document.getElementById(\"textArea" + s_uniqueID + "\");\r\n" +
                                                            "const remLine1 = document.getElementById(\"newline1\");\r\n" +
                                                            "const remLine2 = document.getElementById(\"newline2\");\r\n" +
                                                            "rmQuestion.innerText = \"\";\r\n" +
                                                            "remLine1.remove();\r\n" +
                                                            "remLine2.remove();\r\n" +
                                                            "remElement.remove();\r\n" +
                                                            "}\r\n};";

                            }
                            else
                            {
                                if (s_leadingQuestionAnswer.ToLower() == "no")
                                {

                                    GlobalVars.inlineScript += "function noradioFunction" + s_uniqueID + "(){\r\n " +
                            "if(document.getElementById(\"textArea" + s_uniqueID + "\") ===null)\r\n{\r\n" +
                            "if(document.getElementById(\"flexInputDefault" + s_uniqueID + "\").innerText !== \"" + s_LeadingQuestionText + "\"){ \r\n" +
                            "document.getElementById(\"flexInputDefault" + s_uniqueID + "\").innerText = \"" + s_LeadingQuestionText + "\"; \r\n}\r\n" +
                            "const eli =  document.getElementById(\"flexInputDefault" + s_uniqueID + "\");\r\n" +
                            "const newline = document.createElement(\"br\");\r\n" +
                            "newline.id = \"newline1\";\r\n" +
                            "const newline2 = document.createElement(\"br\");\r\n" +
                            "newline2.id = \"newline2\";\r\n" +
                            "const textArea = document.createElement(\"textarea\");\r\n" +
                            "textArea.id = \"textArea" + s_uniqueID + "\";\r\n" +
                            "textArea.rows = \"2\";\r\n" +
                            "textArea.className =  \"form-control\";\r\n" +
                            "textArea.name = \"TextInput-" + s_uniqueID + "\";\r\n" +
                            "eli.append(newline, newline2, textArea);\r\n" +
                            "}\r\n};\r\n" +

                            "function radioFunction" + s_uniqueID + "(){\r\n " +
                            "if(document.getElementById(\"textArea" + s_uniqueID + "\") !==null)\r\n{\r\n" +
                            "const rmQuestion = document.getElementById(\"flexInputDefault" + s_uniqueID + "\");\r\n" +
                            "const remElement = document.getElementById(\"textArea" + s_uniqueID + "\");\r\n" +
                            "const remLine1 = document.getElementById(\"newline1\");\r\n" +
                            "const remLine2 = document.getElementById(\"newline2\");\r\n" +
                            "rmQuestion.innerText = \"\";\r\n" +
                            "remLine1.remove();\r\n" +
                            "remLine2.remove();\r\n" +
                            "remElement.remove();\r\n" +
                            "}\r\n};";
                                }
                            }
                        }
                    }
                    else
                    {
                        result = "<fieldset>\r\n" +
                                   "<div>\r\n" +
                                       "<input class=\"form-check-input\" type=\"radio\" name=\"YesNo-" + s_uniqueID + "\" id=\"flexRadioDefault"+ s_uniqueID + "No\" value=\"No\"> \r\n" +
                                       "<label class=\"form-check-label\" for=\"flexRadioDefault" + s_uniqueID + "No\">\r\n No\r\n </label>\r\n" +
                                   "</div>\r\n" +
                                   "<div>\r\n" +
                                       "<input class=\"form-check-input\" type=\"radio\" name=\"YesNo-" + s_uniqueID + "\" id=\"flexRadioDefault" + s_uniqueID + "Yes\" value=\"Yes\" required>\r\n" +
                                       "<label class=\"form-check-label\" for=\"flexRadioDefault" + s_uniqueID + "Yes\">\r\nYes\r\n </label>\r\n" +
                                       "<div class=\"invalid-feedback\">\r\n\tPlease select a valid option.\r\n</div>\r\n" +
                                   "</div>\r\n" +
                                 "</fieldset>";

                    }
                    break;
                //Multiple Choice
                case "4":

                    IEnumerable<string> s_options = GetDropDownList(Int32.Parse(s_answerGroupID));
                    string options = "<option selected></option>\r\n";
                    foreach (string option in s_options)
                    {
                        options += "<option value=" + option + ">" + option + "</option>\r\n";
                    }
                    result = "<select class=\"form-select\" id=\"Control" + s_uniqueID + "\" name=\"MultipleChoice-" + s_uniqueID + "\" size=\"4\" required>\r\n " + options + "</select>\r\n"+
                        "<div class=\"invalid-feedback\">\r\n\tPlease select a valid option.\r\n</div>"+
                        "<br>\r\n";

                    break;
                //Date
                case "5":
                    result = "<input id=\"Control" + s_uniqueID + "\" class=\"form-control\" type=\"date\" name=\"DateInput-" + s_uniqueID + "\" >\r\n<br>\r\n" +
                         "<div class=\"invalid-feedback\">\r\n\tPlease insert a valid date.\r\n</div>";
                                       
                    break;
                //File Upload
                case "7":
                    result = "<input  class=\"form-control\" type=\"file\" id=\"Control" + s_uniqueID + "\" name=\"FileUpload-" + s_uniqueID + "\" accept=\"image/png, image/jpeg\" >\r\n<br>\r\n" + "<div class=\"invalid-feedback\">\r\n\tPlease upload a file.\r\n</div>";

                    break;
                //Range
                case "8":
                    result =  "<div class=\"rate py-3 mt-3\"> " +
                              "<div class=\"form-control rating\">"+
                              "<input type=\"radio\" name=\"RatingInput-" + s_uniqueID + "\" value=\"5\" id=\"1-"+s_uniqueID+"\" required>"+
                              "<label for=\"1-"+s_uniqueID+ "\">&#9734;</label>" +
                              "<input type=\"radio\" name=\"RatingInput-" + s_uniqueID + "\" value=\"4\" id=\"2-"+s_uniqueID+"\" required>" +
                              "<label for=\"2-"+s_uniqueID+ "\">&#9734;</label>" +
			                  "<input type=\"radio\" name=\"RatingInput-" + s_uniqueID + "\" value=\"3\" id=\"3-"+s_uniqueID+"\" required>"+
                              "<label for=\"3-"+s_uniqueID+ "\">&#9734;</label>" +
			                  "<input type=\"radio\" name=\"RatingInput-" + s_uniqueID + "\" value=\"2\" id=\"4-"+s_uniqueID+"\" required>"+
                              "<label for=\"4-"+s_uniqueID+ "\">&#9734;</label>" +
			                  "<input type=\"radio\" name=\"RatingInput-" + s_uniqueID + "\" value=\"1\" id=\"5-" + s_uniqueID + "\" required>" +
                              "<label for=\"5-" + s_uniqueID + "\">&#9734;</label>" +
                              "</div>";
                    break;
                //DropDown
                case "9":
                    IEnumerable<string> s2_options = GetDropDownList(Int32.Parse(s_answerGroupID));
                    string options2 = "<option selected></option>\r\n";
                    foreach (string option in s2_options)
                    {
                        options2 += "<option value=" + option + ">" + option + "</option>\r\n";
                    }
                    result = "<select class=\"form-select\" id=\"Control" + s_uniqueID + "\" name=\"DropDown-" + s_uniqueID + "\" required>\r\n " + options2 + "</select>\r\n" + "<div class=\"invalid-feedback\">\r\n\tPlease select an option.\r\n</div>";
                    break;

                //TextArea
                case "6":
                    result = "<textarea rows =\"2\" class=\"form-control\" name=\"TextArea-" + s_uniqueID + "\" id=\"Control" + s_uniqueID + "\" required></textarea>\r\n" + "<div class=\"invalid-feedback\">\r\n\tPlease insert valid text .\r\n</div>";

                    break;
                case "10":
                    
                    IEnumerable<string> s3_options = GetDropDownList(Int32.Parse(s_answerGroupID));
                    string options3 = "<option selected></option>\r\n";
                    foreach (string option in s3_options)
                    {
                        options3 += "<option value=" + option + ">" + option + "</option>\r\n";
                    }
                    result = "<select class=\"form-select\" id=\"Control" + s_uniqueID + "\" name=\"MultipleChoice-" + s_uniqueID + "\" size=\"4\" multiple=\"multiple\" required>\r\n " + options3 + "</select>\r\n" +
                        "<div class=\"invalid-feedback\">\r\n\tPlease select a valid option.\r\n</div>" +
                        "<br>\r\n";

                    break;

                default:
                    result = "<textarea rows =\"2\" class=\"form-control\" name=\"TextArea-" + s_uniqueID + "\" id=\"Control" + s_uniqueID + "\" required></textarea>\r\n" + "<div class=\"invalid-feedback\">\r\n\tPlease insert valid text .\r\n</div>";
                    break;
            }
            return result;
        }

        public IEnumerable<string> GetDropDownList(int s_answerGroupID)
        {
            List<string> dropItems = new();
            List<LAnswerOption> answerOptions = new();

            var dbDropItems = _dbrepo.GetListOfItems(s_answerGroupID);

            if (dbDropItems != null)
            {
                foreach (var dbItem in dbDropItems.Result)
                {
                    answerOptions.Add(new LAnswerOption { Sequence = dbItem.Sequence, Value = dbItem.Value, Score = dbItem.Score });
                }
                //Order answerOptions by squence
                List<LAnswerOption> sortedOrder = answerOptions.OrderBy(a => a.Sequence).ToList();

                foreach (var answerItemOption in sortedOrder)
                {
                    dropItems.Add(answerItemOption.Value);
                }

            }
            else
            {

                return dropItems;
            }
            return dropItems.ToArray();
        }

        public string GetReferenceLink(byte[] data, int RequestID)
        {
            string response = "";
            return response;
        }

        public string CreateReferenceLink(Guid? uniqueID)
        {
            string linkUrlString = _configuration.GetConnectionString("ReferenceCheckLink");
            string? uniqRef = uniqueID.ToString();
            uniqRef = uniqRef!.ToUpper();
            string linkresponse = linkUrlString + "uniqId?uniqId=" + uniqRef.ToString();
            return linkresponse;
        }

        public int CreateRequest(LRequestModel reqModel)
        {
            int request_id = 0;

            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };


            using (HttpClient client = new(clientHandler))
            {
                var endpoint = new Uri(_configuration.GetConnectionString("RequestURL"));
                var newReqModel = JsonConvert.SerializeObject(reqModel);
                var payload = new StringContent(newReqModel, Encoding.UTF8, "application/json");

                var result = client.PostAsync(endpoint + "Request", payload).Result.Content.ReadAsStringAsync().Result;
                dynamic resultJson = JObject.Parse(result);
                request_id = Convert.ToInt16(resultJson.id);

            }

            return request_id;
        }

        public byte[] ReferenceCheck(Guid? guid)
        {
            byte[] formData = _dbrepo.GetReferenceForm(guid);
            return formData;
        }

        public Guid? GetReferenceFormGUUID(int RequestID)
        {
            Guid? guidString = _dbrepo.GetReferenceFormGUUID(RequestID);
            return guidString;
        }

        public List<Question> GetAllQuestions() => _dbrepo.GetAllQuestions();

        public List<Question> GetQuestionsBySetID(int QuestionSetID)
        {
            var response = _dbrepo.GetQuestionsBySetID(QuestionSetID);
            return response.Result.ToList();
        }

        public List<Question> GetQuestionsByText(string text)
        {
            return _dbrepo.GetQuestionsByText(text);
        }

        public Question UpdateQuestion(Question question)
        {
            return _dbrepo.UpdateQuestion(question);
        }

        public Question CreateQuestion(Question question)
        {
            return _dbrepo.CreateQuestion(question);
        }

        public bool DeleteQuestion(int id)
        {
            return _dbrepo.DeleteQuestion(id);
        }

        public Question GetQuestionById(int questionID)
        {
            return _dbrepo.GetQuestionById(questionID);
        }

        public Referee GetRefereeInfo(int refereeId)
        {
            return _dbrepo.GetRefereeInfo(refereeId);

        }

        public ReferenceExtendedData GetReferenceExtendedData(int requestId)
        {
            return _dbrepo.GetReferenceExtendedData(requestId);
        }

     public string NoConsentToReference(LNoConsentModel noCModel)
        {
            string response = string.Empty;
            if( _dbrepo.UpdateReferenceForm(noCModel))
            {
                response = "updated success";
            }
            else
            {
                response = "failed to update";
            }
            return response;
            
        }

        public bool IsFormStillValid(Guid targetGuid)
        {
            return _dbrepo.IsFormStillValid(targetGuid);
        }

        public string GetAManuallyFillForm(string requestKey)
        {
            int requestId = _dbrepo.GetRequestIDFromRequestKey(requestKey);
            if( requestId > 0)
            {
                //Get Form by Request ID
                Guid? uniqId = GetReferenceFormGUUID(requestId);
                byte[] formData = ReferenceCheck(uniqId);
                string formDataS = System.Text.Encoding.UTF8.GetString(formData);
                return formDataS;
            }
            return string.Empty;
        }


        #endregion


    }
}
