using BackendAPI.Database;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.V8;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IDashboard _dservice;
        private IQuestionnaire _qservice;
        private IAnswer _aservice;
        private readonly IDatabaseRepo _dbcontext;
        private readonly IRequests _reqserve;

        public AnswersController(IDashboard dservice, IQuestionnaire qservice, IAnswer aservice, IDatabaseRepo dbcontext, IRequests reqserve)
        {
            _dservice = dservice;
            _qservice = qservice;
            _aservice = aservice;
            _dbcontext = dbcontext;
            _reqserve = reqserve;

        }

        [HttpPost]
        public void PostAnswers([FromForm] Object clientAnswer)
        {


            string formGuid = string.Empty;
            string targetRequestKey = string.Empty;
            List<LRefereeQAModel> lRefereeQAModel = new List<LRefereeQAModel>();

            if (this.Request.Form.Count > 0)
            {
                LRefereeQAModel lReferenceResponseModel = new LRefereeQAModel();

                StringValues qSet;
                StringValues rKey;
                StringValues RefereeName;
                StringValues RefereeFirstName;
                StringValues RefereeLastName;
                StringValues RefereeEmail;
                StringValues RefereeCountry;
                StringValues RefereeMobileNumber;
                StringValues RefereeCompanyName;
                StringValues ReferenceType;
                StringValues RefereeJobTitle;
                StringValues RefereeSpeciality;
                StringValues reqAnswer;

                StringValues CandidateName;
                StringValues CandidateEmail;
                StringValues CandidateFirstName;
                StringValues CandidateLastName;
                StringValues RefereeID;
                StringValues AnswerWeight;

                StringValues DateConducted;
                StringValues RequestId;
                StringValues FormGUID;


                //int? AnswerGroupID = 0;
                //int? AnswerScore = 0;

                Request.Form.TryGetValue("QuestionSetId", out qSet);
                lReferenceResponseModel.QuestionnaireSetId = Convert.ToInt32(qSet.First());
                string qSetName = _dservice.GetQustionSetInfo(lReferenceResponseModel.QuestionnaireSetId);
                Request.Form.TryGetValue("RequestKey", out rKey);
                lReferenceResponseModel.RequestKey = (rKey[0]);
                targetRequestKey = (rKey[0]);
                Request.Form.TryGetValue("RefereeName", out RefereeName);
                lReferenceResponseModel.RefereeName = Convert.ToString(RefereeName);
                Request.Form.TryGetValue("RefereeFirstName", out RefereeFirstName);
                lReferenceResponseModel.RefereeFirstName = Convert.ToString(RefereeFirstName);
                Request.Form.TryGetValue("RefereeLastName", out RefereeLastName);
                lReferenceResponseModel.RefereeLastName = Convert.ToString(RefereeLastName);
                Request.Form.TryGetValue("RefereeEmail", out RefereeEmail);
                lReferenceResponseModel.RefereeEmail = Convert.ToString(RefereeEmail);
                Request.Form.TryGetValue("RefereeCountry", out RefereeCountry);
                lReferenceResponseModel.RefereeCountry = Convert.ToString(RefereeCountry);
                Request.Form.TryGetValue("RefereeMobileNumber", out RefereeMobileNumber);
                lReferenceResponseModel.RefereeMobile = Convert.ToString(RefereeMobileNumber);
                Request.Form.TryGetValue("RefereeCompanyName", out RefereeCompanyName);
                lReferenceResponseModel.RefereeCompanyName = Convert.ToString(RefereeCompanyName);
                Request.Form.TryGetValue("ReferenceType", out ReferenceType);
                lReferenceResponseModel.ReferenceType = Convert.ToString(ReferenceType);
                Request.Form.TryGetValue("RefereeJobTitle", out RefereeJobTitle);
                lReferenceResponseModel.RefereeJobTitle = Convert.ToString(RefereeJobTitle);
                Request.Form.TryGetValue("RefereeSpeciality", out RefereeSpeciality);
                lReferenceResponseModel.RefereeSpeciality = Convert.ToString(RefereeSpeciality);
                Request.Form.TryGetValue("CandidateName", out CandidateName);
                lReferenceResponseModel.CandidateName = Convert.ToString(CandidateName);
                Request.Form.TryGetValue("DateConducted", out DateConducted);
                Request.Form.TryGetValue("RequestID", out RequestId);
                lReferenceResponseModel.RequestID = Convert.ToInt32(RequestId);
                Request.Form.TryGetValue("CandidateEmail", out CandidateEmail);
                lReferenceResponseModel.CandidateEmail = Convert.ToString(CandidateEmail);
                Request.Form.TryGetValue("CandidateFirstName", out CandidateFirstName);
                lReferenceResponseModel.CandidateFirstName = Convert.ToString(CandidateFirstName);
                Request.Form.TryGetValue("CandidateLastName", out CandidateLastName);
                lReferenceResponseModel.CandidateLastName = Convert.ToString(CandidateLastName);
                Request.Form.TryGetValue("RefereeID", out RefereeID);
                lReferenceResponseModel.RefereeID = Convert.ToInt32(RefereeID);
                Request.Form.TryGetValue("AnswerWeight", out AnswerWeight);     //Get answerWeight from the DB
                Request.Form.TryGetValue("ReferenceUUID", out FormGUID); //Get the form Guid

                formGuid = FormGUID.ToString();   //-

                foreach (var keyVar in this.Request.Form.Keys)
                {
                    string keyVarString = "";
                    int keyVarNumber = 0;
                    string keyVarAnswerType = "";
                    if (keyVar != null)
                    {
                        keyVarString = keyVar.ToString();
                        if (keyVarString.Contains("-"))
                        {
                            keyVarString = keyVarString.Replace("-", "");
                            keyVarNumber = Convert.ToInt32(Regex.Replace(keyVarString, "[A-Za-z]", ""));
                            keyVarAnswerType = Regex.Replace(keyVarString, "[0-9]", "");


                        }
                    }

                    //Get reference Guid
                    if (keyVar!.ToLower() == "referenceuuid")
                    {

                    }


                    if (
                        keyVar!.ToLower() != "questionsetid" &&
                        keyVar.ToLower() != "requestkey" &&
                        keyVar.ToLower() != "refereename" &&
                        keyVar.ToLower() != "refereefirstname" &&
                        keyVar.ToLower() != "refereelastname" &&
                        keyVar.ToLower() != "refereeemail" &&
                        keyVar.ToLower() != "refereecountry" &&
                        keyVar.ToLower() != "refereemobilenumber" &&
                        keyVar.ToLower() != "refereecompanyname" &&
                        keyVar.ToLower() != "referencetype" &&
                        keyVar.ToLower() != "refereejobtitle" &&
                        keyVar.ToLower() != "refereespeciality" &&
                        keyVar.ToLower() != "candidatename" &&
                        keyVar.ToLower() != "dateconducted" &&
                        keyVar.ToLower() != "requestid" &&
                        keyVar.ToLower() != "candidatefirstname" &&
                        keyVar.ToLower() != "candidatelastname" &&
                        keyVar.ToLower() != "candidateemail" &&
                        keyVar.ToLower() != "refereeid" &&
                        keyVar.ToLower() != "answerweight" &&
                        keyVar.ToLower() != "referenceuuid"


                        )
                    {
                        lRefereeQAModel.Add(new LRefereeQAModel()
                        {

                            DateCreated = DateTime.Now,

                            CandidateName = lReferenceResponseModel.CandidateName,

                            CandidateFirstName = lReferenceResponseModel.CandidateFirstName,

                            CandidateLastName = lReferenceResponseModel.CandidateLastName,

                            CandidateEmail = lReferenceResponseModel.CandidateEmail,

                            AnswerWeight = lReferenceResponseModel.AnswerWeight,

                            RefereeEmail = lReferenceResponseModel.RefereeEmail,

                            RefereeFirstName = lReferenceResponseModel.RefereeFirstName,

                            RefereeLastName = lReferenceResponseModel.RefereeLastName,

                            RefereeSpeciality = lReferenceResponseModel.RefereeSpeciality,

                            RefereeJobTitle = lReferenceResponseModel.RefereeJobTitle,

                            RefereeCompanyName = lReferenceResponseModel.RefereeCompanyName,

                            RefereeCountry = lReferenceResponseModel.RefereeCountry,

                            RefereeMobile = lReferenceResponseModel.RefereeMobile,

                            RefereeName = lReferenceResponseModel.RefereeName,

                            ReferenceType = lReferenceResponseModel.ReferenceType,

                            QuestionnaireSetId = Convert.ToInt32(qSet),

                            QuestionnaireSetName = qSetName,

                            QuestionID = keyVarNumber,

                            RequestKey = (rKey[0]!),

                            Question = _dservice.GetQuestionById(keyVarNumber),

                            Answer = (Request.Form.TryGetValue(keyVar.ToString(), out reqAnswer) ? reqAnswer.ToString() : "No answer text found"),

                            CreationDate = DateTime.Now,

                            AnswerType = keyVarAnswerType,

                            RequestID = lReferenceResponseModel.RequestID,

                            RefereeID = lReferenceResponseModel.RefereeID

                        });

                    }

                }


            }


            bool answerSave = _dservice.SaveRefereeAnswers(lRefereeQAModel);
            Thread.Sleep(3000);

            if (answerSave)
            {
            
                //Update Requests
              
                    bool iState = _reqserve.UpdateRequestStatus(targetRequestKey!, "Reference");
                   if(iState)
                    {
                    string retDataS = "Saved!!";
                    Console.WriteLine(retDataS);
                    }


            }
            else
            {
                string retDataS = "Not Saved to DB!!!";
                Console.WriteLine(retDataS);
            }

        }

        [HttpGet("{RequestKey}")]
        public ContentResult GetReferenceAnswers(string RequestKey)
        {
            return base.Content(_aservice.GetReferenceAnswers(RequestKey), "text/html");
        }

        [HttpPost]
        public void PostEditedAnswers([FromForm] Object clientAnswer)
        {

            string TargetRequestKey = string.Empty;
            List<EditDBModel> UpdatedAnswers = new List<EditDBModel>();
            string formGuid = string.Empty;
            if (this.Request.Form.Count > 0)
            {


                StringValues RequestKey;
               
                Request.Form.TryGetValue("RequestKey", out RequestKey);
                TargetRequestKey = RequestKey.ToString();


                        var list = Request.Form.ToList();
                        
                        foreach (var item in list)
                        {
                            if(item.Key.ToLower() == "requestkey") continue;
                            if (item.Key.ToLower() == "refereefirstname") continue;
                            if (item.Key.ToLower() == "refereelastname" ) continue;
                            if (item.Key.ToLower() == "refereeemail") continue;
                            if (item.Key.ToLower() == "refereecountry") continue;
                            if (item.Key.ToLower() == "refereemobilenumber") continue;
                            if (item.Key.ToLower() == "refereecompanyname") continue;
                            if (item.Key.ToLower() == "referencetype") continue;
                            if (item.Key.ToLower() == "refereejobtitle" ) continue;
                            if (item.Key.ToLower() == "refereespeciality") continue;

                            string questionId = string.Empty;
                            string wholeKey = item.Key.ToLower();
                    if (wholeKey.Contains('-'))
                    {
                        string[] getKey = wholeKey.Split(new char[] { '-' });
                        if (getKey.Length > 0)
                        {
                            questionId = getKey[1];
                            if (questionId.Contains("'"))
                            {
                                questionId = questionId.Trim(new Char[] { '\'' });
                            }

                        }
                        UpdatedAnswers.Add(new EditDBModel()
                        {
                            RequestKey = RequestKey!,
                            QuestionID = questionId,
                            Answer = item.Value.ToString()

                        });
                    }
               
                           
            }
            
            }

           // Save edited answers
            bool isSaved = _dbcontext.SaveEditedRefereeAnswers(UpdatedAnswers);

            //Update Requests
            if (isSaved)
            {
                bool iState = _reqserve.UpdateRequestStatus(TargetRequestKey, "Approved");
            }


        }
        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------
        [HttpGet("id")]
        public List<LRefereeEditQAModel> GetExistingReferenceForEdit(string id)
        {
            List<LRefereeEditQAModel> answersRef =  _aservice!.GetExistingReferenceForEdit(id);
            return answersRef;
           
        }
                
        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------
        [HttpGet("id")]
        public string GetUnfilledReference(string id)
        {
            string returnString = "NotFiled";
            return returnString;

        }

        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------
        [HttpGet("id")]
        public List<AnswerOptionsModel> GetAnswerOptions(int questionSetId)
        {
            return _dbcontext.GetAnswerOptions(questionSetId);

        }

    }
}
