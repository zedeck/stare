using BackendAPI.Constants;
using BackendAPI.Global;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using System.Net.Mime;

namespace BackendAPI.Services
{
    public class AnswerService : IAnswer
    {
        private readonly IConfiguration _config;
        private readonly IDatabaseRepo _dbserve;

        public AnswerService(IDatabaseRepo dbserve, IConfiguration config)
        {

            _config = config;
            _dbserve = dbserve;
        }
        

        public string CreateEditingForm(List<LRefereeEditQAModel> answers, int page)
        {

            //get Connection Strings
            string postingAnswerUrl = _config.GetConnectionString("PosteEditedAnswersEndPoint").ToString();
           
            //Get referee information
            EditRefereeInfo refereeInfo = new EditRefereeInfo();
           foreach(var refInfo in answers)
            {
              refereeInfo.RefereeID = refInfo.RefereeID;
                refereeInfo.RefereeName = refInfo.RefereeName;
                refereeInfo.RefereeFirstName = refInfo.RefereeFirstName;
                refereeInfo.RefereeLastName = refInfo.RefereeLastName;
                refereeInfo.RefereeEmail = refInfo.RefereeEmail;
                refereeInfo.RefereeCompanyName = refInfo.RefereeCompanyName;
                refereeInfo.RefereeCountry = refInfo.RefereeCountry;
                refereeInfo.RefereeCountry = refereeInfo.RefereeCountry;
                refereeInfo.RefereeJobTitle = refInfo.RefereeJobTitle;
                refereeInfo.RefereeMobile = refInfo.RefereeMobile;
                refereeInfo.RefereeSpeciality = refInfo.RefereeSpeciality;
                break;
            }

           //Get candidate information
           EditCandidateInfo candidateInfo = new EditCandidateInfo();
           foreach(var refInfo in answers)
            {
                candidateInfo.CandidateFirstName = refInfo.CandidateFirstName;
                candidateInfo.CandidateLastName = refInfo.CandidateLastName;
                candidateInfo.CandidateName = refInfo.CandidateName;
                candidateInfo.CandidateEmail = refInfo.CandidateEmail;

            }

           //Get general Information
           EditQAGeneric genericInfo = new EditQAGeneric();
            foreach(var refInfo in answers)
            {
                genericInfo.RequestKey = refInfo.RequestKey;
                genericInfo.RequestID = refInfo.RequestID;
                genericInfo.ReferenceType = refInfo.ReferenceType;
                genericInfo.QuestionDate = refInfo.QuestionDate;
                genericInfo.QuestionnaireSetId = refInfo.QuestionnaireSetId;
                genericInfo.QuestionnaireSetName = refInfo.QuestionnaireSetName;
            }

            //Get questions and answers
            List<EditQAs> qasInfo = new List<EditQAs>();
            foreach(var refInfo in answers)
            {
                qasInfo.Add(new EditQAs {
                    Question = refInfo.Question,
                    AnswerType = refInfo.AnswerType,
                    Answer = refInfo.Answer,
                    AnswerWeight = refInfo.AnswerWeight,
                    AnswerDate = refInfo.AnswerDate,
                    QuestionID = refInfo.QuestionID
                    });

            }

            string agentName = "Godwill"; //string.Empty;   //Get login name here
            string agentEmail = "godwillm@mie.co.za"; // string.Empty;
            string agentSurname = "Makhubela"; // string.Empty;
            string agentMobile = "081592542"; // string.Empty;
            string agentSpeciality = "Verification";  //string.Empty;
            string refenceType = genericInfo.ReferenceType.ToString();
            string requestKey = genericInfo.RequestKey.ToString();


            //--------------------
            //Create form
            string formDocument = EditFormTemplate.DOCUMENT_MAIN.ToString();
            string formHead = EditFormTemplate.HEAD_SECTION.ToString();
            string formBody =  EditFormTemplate.BODY_SECTION.ToString() ;
            string formStylings = EditFormTemplate.STYLING_SECTION.ToString();
            string formScripts  = EditFormTemplate.SCRIPTS_SECTION.ToString();
            string formContent = EditFormTemplate.CONTENT_SECTION.ToString();
            formContent = formContent.Replace("{REQUESTKEY}", genericInfo.RequestKey);

            //-----------
            formContent = 
                "<form method=\"POST\" class=\"w-100 rounded-1 p-4 border bg-secondary text-dark bg-opacity-10 needs-validation\"\r\n        action=\""+ postingAnswerUrl + "\" enctype=\"multipart/form-data\" novalidate>"+
                //"<h3 class=\"fw-bold text-dark \">Agent details:</h3>\r\n" +
                "<br>\r\n"+
                "<div class=\"row my-8\">\r\n"+
                    "<div class=\"col-md-6 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"RefereeFirstNameControl\" name=\"RefereeFirstName\" class=\"form-control\" value=\""+agentName+"\" required />\r\n"+
                            "<label class=\"form-label\" for=\"RefereeFirstNameControl\">First name</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\nPlease provide a valid first name.\r\n</div>\r\n"+
                        "</div>\r\n"+
                    "</div>\r\n"+
                    "<div class=\"col-md-6 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"RefereeLastNameControl\" name=\"RefereeLastName\" class=\"form-control\" value=\""+agentSurname+"\" required />\r\n"+
                            "<label class=\"form-label\" for=\"RefereeLastNameControl\">Last name</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\nPlease provide a valid last name.\r\n</div>\r\n"+
                        "</div>\r\n"+
                    "</div>\r\n"+
                 "</div>\r\n"+
                 "<br>\r\n"+
                 "<div class=\"row my-8\">\r\n"+
                    "<div class=\"col-md-6 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"email\" id=\"RefereeEmailControl\" name=\"RefereeEmail\" class=\"form-control\" value=\""+agentEmail+"\" required />\r\n"+
                            "<label class=\"form-label\" for=\"RefereeEmailControl\">Email</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\nPlease provide a valid email.\r\n</div>\r\n"+
                         "</div>\r\n"+
                     "</div>\r\n"+
                     "<div class=\"col-md-2 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                        "<input type=\"tel\" id=\"RefereeCountryControl\" name=\"RefereeCountry\" class=\"form-control\"\r\n value=\"South Africa\" required />\r\n"+
                        "<label class=\"form-label\" for=\"RefereeCountryControl\">Country</label>\r\n"+
                            "<div class=\"invalid-feedback\">\r\nPlease provide a valid country name.\r\n</div>\r\n"+
                        "</div>\r\n"+
                     "</div>\r\n"+
                     "<div class=\"col-md-4 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"RefereeMobileNumberControl\" name=\"RefereeMobileNumber\" class=\"form-control\"\r\n value=\""+agentMobile+"\" required />\r\n"+
                            "<label class=\"input-group-addon\" for=\"RefereeMobileNumberControl\">Mobile number</label>\r\n"+
                            "<div class=\"invalid-feedback\">\r\nPlease provide a valid mobile number.\r\n</div>\r\n"+
                        "</div>\r\n"+
                     "</div>\r\n"+
                 "</div>\r\n"+
                 "<br>\r\n"+
                 "<div class=\"row my-8\">\r\n"+
                    "<div class=\"col-md-3 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"RefereeCompanyNameControl\" name=\"RefereeCompanyName\" class=\"form-control\"\r\n value=\"Mettus\" required />\r\n"+
                            "<label class=\"form-label\" for=\"RefereeCompanyNameControl\">Company name</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\nPlease provide a valid company name.\r\n</div>\r\n"+
                        "</div>\r\n"+
                    "</div>\r\n"+
                    "<div class=\"col-md-3 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"ReferenceTypeControl\" name=\"ReferenceType\" class=\"form-control\" value=\""+refenceType+"\" required>\r\n"+
                            "<label class=\"form-label\" for=\"ReferenceTypeControl\">Reference type</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\nPlease provide a valid reference type.\r\n</div>\r\n"+
                        "</div>\r\n"+
                    "</div>\r\n"+
                    "<div class=\"col-md-3 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"RefereeJobTitleControl\" name=\"RefereeJobTitle\" class=\"form-control\" value=\"Agent\"\r\n required />\r\n"+
                            "<label class=\"form-label\" for=\"RefereeJobTitleControl\">Job title</label>\r\n"+
                                "<div class=\"invalid-feedback\">\r\nPlease provide a valid job title.\r\n</div>\r\n"+
                        "</div>\r\n"+
                    "</div>\r\n"+
                    "<div class=\"col-md-3 visually-hidden\">\r\n" +
                        "<div class=\"form-floating\">\r\n\r\n"+
                            "<input type=\"text\" id=\"RefereeSpecialityControl\" name=\"RefereeSpeciality\" class=\"form-control\" value=\""+agentSpeciality+"\" />\r\n"+
                            "<label class=\"form-label\" for=\"RefereeSpecialityControl\">Speciality</label>\r\n</div>\r\n"+
                        "</div>\r\n"+
                     "</div>\r\n"+
                     "<br>\r\n"+
                     "<br>\r\n\r\n\r\n"+
                     "<hr class=\"my-1\">\r\n"+
                     "<br>\r\n"+
                     "<h3 class=\"fw-bold text-dark\">Reference Questions and Answers</h3>\r\n" +
                      "<input  class=\"form-control visually-hidden\" type=\"text\" id=\"RequestControl\" name=\"RequestKey\" value=\"" + requestKey + "\" ><br> \r\n" +
                      "<input class=\"form-control visually-hidden\" type=\"text\" id=\"PageIndexControl\" name=\"PageIndex\" value=\""+page+"\"><br>\r\n"+
                     "<br>\r\n";
            //Add questions and Answers
            foreach (var qasVar in qasInfo)
            {

                   formContent += "<label class=\"d-block mb-4 fontStyle\" for=\"Control" + qasVar.QuestionID + "\"> \r\n <span class=\"form-label d-block\">" + qasVar.Question + "</span> \r\n" +
                    getHtmlControl(qasVar.AnswerType.ToString(), qasVar.QuestionID, qasVar.Answer) +
                    "</label> \r\n<br>\r\n <hr class=\"my-1\">\r\n <br>"; 
            }
            formContent += " <div class=\"row mb-12\">\r\n          <div class=\"mb-3\">\r\n            <button type=\"submit\" id=\"submitForm\"\r\n              class=\"btn btn-success btn-lg px-3 rounded-3 text-dark\">Approve</button>\r\n            <button type=\"button\" id=\"saveForm\"\r\n              class=\"btn btn-warning btn-lg px-3 rounded-3 ms-3 text-dark\">SaveNow</button>\r\n          </div>\r\n        </div>" +
                           "</div>" +
                            "</form>";
                        
            

            formHead = formHead.Replace("{STYLING_SECTION}",formStylings);
            formBody = formBody.Replace("{CONTENT_SECTION}", formContent);
            formBody = formBody.Replace("{SCRIPTS_SECTION}", formScripts);
            formDocument = formDocument.Replace("{HEAD_SECTION}", formHead);
            formDocument = formDocument.Replace("{BODY_SECTION}", formBody);

            //----------------------
            return formDocument;

        }

        public string GetReferenceAnswers(string RequestKey)
        {
            List<LRefereeEditQAModel> refereeAnswers = new List<LRefereeEditQAModel>();
            var refAnswersFromDb = _dbserve.GetReferenceAnswersByReqKey(RequestKey);
            foreach(var refAnswers in refAnswersFromDb)
            {
                refereeAnswers.Add(refAnswers);

            }
            return CreateEditingForm(refereeAnswers, 1);
           
        }


        public string getHtmlControl(string controlType, int uniqId, string answerValue)
        {
            string result;
            switch (controlType)
            {
                case "NumericInput":
                    result = "<input  class=\"form-control\" type=\"number\" id=\"Control" + uniqId + "\" name=\"NumericInput-" + uniqId + "\" min=\"1\" max=\"100\" value=\"" + answerValue + "\" required> \r\n" +
                       "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid numeric input.\r\n</div>";
                    break;
                case "RatingInput":

                    string[] isSelected = { "", "", "", "", "" };
                    if (answerValue != null) {

                        int index = Convert.ToInt16(answerValue);
                        if(index > 0)
                            isSelected[index - 1] = "checked";

                    }

                    result = "<div class=\"rate py-3 mt-3\"> " +
                            "<div class=\"form-control rating\">" +
                            "<input type=\"radio\" name=\"RatingInput-" + uniqId + "\" value=\"5\" id=\"1-" + uniqId + "\" "+isSelected[4]+" required>" +
                            "<label for=\"1-" + uniqId + "\">&#9734;</label>" +
                            "<input type=\"radio\" name=\"RatingInput-" + uniqId + "\" value=\"4\" id=\"2-" + uniqId + "\" "+isSelected[3]+" required>" +
                            "<label for=\"2-" + uniqId + "\">&#9734;</label>" +
                            "<input type=\"radio\" name=\"RatingInput-" + uniqId + "\" value=\"3\" id=\"3-" + uniqId + "\" "+isSelected[2]+" required>" +
                            "<label for=\"3-" + uniqId + "\">&#9734;</label>" +
                            "<input type=\"radio\" name=\"RatingInput-" + uniqId + "\" value=\"2\" id=\"4-" + uniqId + "\" "+isSelected[1]+" required>" +
                            "<label for=\"4-" + uniqId + "\">&#9734;</label>" +
                            "<input type=\"radio\" name=\"RatingInput-" + uniqId + "\" value=\"1\" id=\"5-" + uniqId + "\" "+isSelected[0]+" required>" +
                            "<label for=\"5-" + uniqId + "\">&#9734;</label>" +
                            "</div>" +
                            "</div>";
                    break;
                case "TextInput":
                    result = "<input  class=\"form-control\" type=\"text\" id=\"Control" + uniqId + "\" name=\"TextInput-" + uniqId + "\" value=\""+answerValue.ToString()+"\" required >   \r\n" +
                               "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid text input.\r\n</div>";
                    break;
                
                case "FileUpload":
                    result = "<input  class=\"form-control\" type=\"file\" id=\"Control" + uniqId + "\" name=\"FileUpload-" + uniqId + "\" accept=\"image/png, image/jpeg\" >\r\n<br>\r\n" + "<div class=\"invalid-feedback\">\r\n\tPlease upload a file.\r\n</div>";
                    break; 
             
                case "YesNo":
                    string IsYesChecked = "checked";
                    string IsNoChecked = "checked";
                    if (answerValue == "Yes")
                    {
                        IsNoChecked = "";
                    }
                    else
                    {
                        IsYesChecked = "";
                    }
                    result = "<fieldset>\r\n" +
                                   "<div>\r\n" +
                                       "<input class=\"form-check-input text-dark\" type=\"radio\" name=\"YesNo-" + uniqId + "\" id=\"flexRadioDefault" + uniqId + "No\" value=\"No\" "+IsNoChecked+"> \r\n" +
                                       "<label class=\"form-check-label text-dark\" for=\"flexRadioDefault" + uniqId + "No\">\r\n No\r\n </label>\r\n" +
                                   "</div>\r\n" +
                                   "<div>\r\n" +
                                       "<input class=\"form-check-input text-dark\" type=\"radio\" name=\"YesNo-" + uniqId + "\" id=\"flexRadioDefault" + uniqId + "Yes\" value=\"Yes\" "+IsYesChecked+" required>\r\n" +
                                       "<label class=\"form-check-label text-dark\" for=\"flexRadioDefault" + uniqId + "Yes\">\r\nYes\r\n </label>\r\n" +
                                       "<div class=\"invalid-feedback\">\r\n\tPlease select a valid option.\r\n</div>\r\n" +
                                   "</div>\r\n" +
                                 "</fieldset>";
                    break;
                case "TextArea":
                    result =  "<input  class=\"form-control\" type=\"text\" id=\"Control" + uniqId + "\" name=\"TextInput-" + uniqId + "\" value=\""+answerValue.ToString()+"\" required > \r\n" +
                                "<div class=\"invalid-feedback\">\r\n\tPlease provide a valid text input.\r\n</div>";

                    break;
                case "Date":
                    result = "<input id=\"Control" + uniqId + "\" class=\"form-control\" type=\"date\" name=\"DateInput-" + uniqId + "\" value=\""+ answerValue+"\" required>\r\n<br>\r\n" +
                "<div class=\"invalid-feedback\">\r\n\tPlease insert a valid date.\r\n</div>";
                    break;
                case "DropDown":
                    var valuet = answerValue.ToString();
                    result = "<select class=\"form-select\" id=\"Control" +uniqId+ "\" name=\"DropDown-" +uniqId+ "\" required>\r\n <option selected> "+valuet+"</option>\r\n </select>\r\n" + "<div class=\"invalid-feedback\">\r\n\tPlease select an option.\r\n</div>";

                    break;
                default:
                    result = "\r\n<input class=\"form-control text-danger\" type=\"text\" id=\"Control"+uniqId+"\" value=\"No Control Found!!!\"/>\r\n";
                    break;

            }


            return result;


        }
        /// <summary>
        /// 
        /// </summary>
        public List<LRefereeEditQAModel> GetExistingReferenceForEdit(string id)
        {
            List<LRefereeEditQAModel> lRefereeEditQAModel = new List<LRefereeEditQAModel>();
            var answers = _dbserve!.GetReferenceAnswersByReqKey(id);

            foreach (var answer in answers) {
                lRefereeEditQAModel.Add(
                    new LRefereeEditQAModel
                    {
                       Answer = answer.Answer,
                       AnswerDate = answer.AnswerDate,
                       AnswerType = answer.AnswerType,
                       AnswerWeight = answer.AnswerWeight,
                       CandidateEmail = answer.CandidateEmail,
                       CandidateFirstName = answer.CandidateFirstName,
                       CandidateLastName = answer.CandidateLastName,
                       CandidateName = answer.CandidateName,
                       Id = answer.Id,
                       Question = answer.Question,
                       QuestionDate = answer.QuestionDate,
                       QuestionID = answer.QuestionID,
                       QuestionnaireSetId = answer.QuestionnaireSetId,
                       QuestionnaireSetName = answer.QuestionnaireSetName,
                       RefereeCompanyName = answer.RefereeCompanyName,
                       RefereeCountry = answer.RefereeCountry,
                       RefereeEmail = answer.RefereeEmail,
                       RefereeFirstName = answer.RefereeFirstName,
                       RefereeID = answer.RefereeID,
                       RefereeJobTitle = answer.RefereeJobTitle,
                       RefereeLastName = answer.RefereeLastName,
                       RefereeMobile = answer.RefereeMobile,
                       RefereeName = answer.RefereeName,
                       RefereeSpeciality = answer.RefereeSpeciality,    
                       ReferenceType = answer.ReferenceType,  
                       RequestID = answer.RequestID,
                       RequestKey = answer.RequestKey
                    }
               );
            }

            return lRefereeEditQAModel;


        }
    }
}
