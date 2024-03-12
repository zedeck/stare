using BackendAPI.Database;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BackendAPI.Services
{
    public class DatabaseService : IDatabaseRepo
    {

        private readonly ERADBContext _context;
        private readonly IConfiguration _configuration;

        public DatabaseService(ERADBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public Task<List<AnswerType>> GetAllAnswerTypes() => _context.AnswerTypes.ToListAsync();



        public async Task<IEnumerable<Question>> GetQuestionsBySetID(int QuestionSetID)
        {

            var qlist = await _context.Questions.Where(a => a.QuestionSetId == QuestionSetID).ToListAsync();
            if (qlist.Any())
            {
                return qlist;
            }
            else
            {
                qlist.Clear();
                return qlist;
            }
        }


        public async Task<List<AnswerOption>> GetListOfItems(int AnswerGroupID)
        {
            return await _context.AnswerOptions.Where(a => a.AnswerGroupId == AnswerGroupID).ToListAsync();
        }


        //Get trailing question
        public Question GetQuestionById(int QuestionID)
        {
            return _context.Questions.Single(a => a.QuestionId == QuestionID);
        }

        public Int32 GetFrontEndControlTypeById(Int32 AnswerGroupID)
        {
            var frontEndControl = _context.AnswerTypes.FirstOrDefault(a => a.FrontEndControlType == AnswerGroupID);
            if (frontEndControl != null)
                return Convert.ToInt32(frontEndControl.FrontEndControlType);
            else
                return 0;
        }

        public QuestionSet GetQuestionSetByID(int QuestionSetId)
        {
            var response = _context.QuestionSets.FirstOrDefault(a => a.QuestionSetId == QuestionSetId);
            return response ?? new QuestionSet();

        }

        public string GetQuestionSetNameByID(int QuestionSetID)
        {
            string returnQSetName = "No question set name found!!";

            var questionSets = _context.QuestionSets.Single(a => a.QuestionSetId == QuestionSetID);

            if (questionSets != null)
                return questionSets.Description!.ToString();
            else
                return returnQSetName;


        }

        public LCandidateModel SP_GetCandidateInfoByReqID(int RequestID)
        {
            //ByPass SSL POLICIES 
            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            LCandidateModel model = new();

            string myDb1ConnectionString = _configuration.GetConnectionString("QADBConnection");
            using (var sqlCon = new SqlConnection(myDb1ConnectionString))
            {
                sqlCon.Open();
                SqlCommand command = new SqlCommand("Exec dbo.usp_GetCandidateDetailsFromRequestKey @RequestId = '" + RequestID + "';", sqlCon); //.ExecuteScalar();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        model.CandidateName = reader.GetString(0);
                    else
                        model.CandidateName = string.Empty;


                    if (!reader.IsDBNull(1))
                        model.CandidateSurname = reader.GetString(1);
                    else
                        model.CandidateSurname = string.Empty;


                    if (!reader.IsDBNull(2))
                        model.CandidateDOB = reader.GetDateTime(2);
                    else
                        model.CandidateDOB = DateTime.MinValue;

                    if (!reader.IsDBNull(3))
                        model.CandidateCell = reader.GetString(3);
                    else
                        model.CandidateCell = string.Empty;


                    if (!reader.IsDBNull(4))
                        model.CandidateEmail = reader.GetString(4);
                    else
                        model.CandidateEmail = string.Empty;


                    if (!reader.IsDBNull(5))
                        model.CandidateID = reader.GetString(5);
                    else
                        model.CandidateID = string.Empty;


                    if (!reader.IsDBNull(6))
                        model.CandidatePassport = reader.GetString(6);
                    else
                        model.CandidatePassport = string.Empty;

                    //model.SigEmail = reader.GetString(7);
                    //model.SigName = reader.GetString(8);
                }
                sqlCon.Close();
            }
            return model;

        }

        public LCandidateModel SP_GetCandidateInfo(string RemoteKey)
        {
            //ByPass SSL POLICIES 
            HttpClientHandler clientHandler = new()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };


            LCandidateModel model = new();

            string myDb1ConnectionString = _configuration.GetConnectionString("QADBConnection");

            using (var sqlCon = new SqlConnection(myDb1ConnectionString))
            {
                sqlCon.Open();
                SqlCommand command = new SqlCommand("Exec dbo.usp_GetCandidateDetails @RemoteKey = '" + RemoteKey + "';", sqlCon); //.ExecuteScalar();
                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        model.CandidateName = reader.GetString(0);
                    else
                        model.CandidateName = string.Empty;


                    if (!reader.IsDBNull(1))
                        model.CandidateSurname = reader.GetString(1);
                    else
                        model.CandidateSurname = string.Empty;


                    if (!reader.IsDBNull(2))
                        model.CandidateDOB = reader.GetDateTime(2);
                    else
                        model.CandidateDOB = DateTime.MinValue;

                    if (!reader.IsDBNull(3))
                        model.CandidateCell = reader.GetString(3);
                    else
                        model.CandidateCell = string.Empty;


                    if (!reader.IsDBNull(4))
                        model.CandidateEmail = reader.GetString(4);
                    else
                        model.CandidateEmail = string.Empty;


                    if (!reader.IsDBNull(5))
                        model.CandidateID = reader.GetString(5);
                    else
                        model.CandidateID = string.Empty;


                    if (!reader.IsDBNull(6))
                        model.CandidatePassport = reader.GetString(6);
                    else
                        model.CandidatePassport = string.Empty;

                    //model.SigEmail = reader.GetString(7);
                    //model.SigName = reader.GetString(8);
                }
                sqlCon.Close();
            }
            return model;
        }


        //-----------------
        //Trailing Question 
        //-----------------
        public string? GetTrailingQText(int? LeadingQuestionID)
        {
            string? question = "";
            var dbresponse = _context.Questions.Where(c => c.LeadingQuestion == LeadingQuestionID);

            if (dbresponse != null)
            {
                foreach (var item in dbresponse)
                {

                    question = item.QuestionText;
                }
            }
            else
            {
                question = null;
            }

            return question;


        }

        public int? GetTrailingQID(string? TrailingQuestionText, int? LeadingQuestionID)
        {
            int? trailingID = 0;

            var dbresponse = _context.Questions.Where(d => d.QuestionText == TrailingQuestionText).Where(e => e.LeadingQuestion == LeadingQuestionID);
            if (dbresponse != null)
            {
                foreach (var d in dbresponse)
                {
                    trailingID = d.QuestionId;
                }
            }
            return trailingID;
        }

        public string? GetTrailingQAnswer(int TrailingQuestionID)
        {
            string? trailingAnswer = "";
            var dpresponse = _context.Questions.Where(f => f.QuestionId == TrailingQuestionID);
            if (dpresponse != null)
            {
                foreach (var d in dpresponse)
                {
                    trailingAnswer = d.LeadingQuestionAnswer!.ToString();
                }

            }
            return trailingAnswer;

        }

        public string CreateReferenceForm(LReferenceForm referenceForm)
        {


            ReferenceForm form = new()
            {
                UniqueId = referenceForm.UniqueId,
                RequestId = referenceForm.RequestId,
                Link = referenceForm.Link,
                FormData = referenceForm.FormData,
                CreationDate = referenceForm.CreationDate,
                ExpiryDate = referenceForm.ExpiryDate,
                IsExpired = referenceForm.IsExpired,
                IsCompleted = referenceForm.IsCompleted,
                IsActive = referenceForm.IsActive,
                Status = referenceForm.Status,
                CreatedBy = referenceForm.CreatedBy!
            };

            _context.ReferenceForms.Add(form);
            _context.SaveChanges();
            return form.Link;

        }

        public byte[] GetReferenceForm(Guid? uniqueID)
        {
            var reference = _context.ReferenceForms.Where(c => c.UniqueId == uniqueID).FirstOrDefault();
            return reference != null ? reference.FormData : Array.Empty<byte>();
        }

        public bool UpdateReferenceForm(LReferenceForm referenceForm)
        {
            throw new NotImplementedException();
        }

        public bool DeleteReferenceForm(int RequestID)
        {
            throw new NotImplementedException();
        }

        public Guid? GetReferenceFormGUUID(int RequestID)
        {

            var dbresponse = _context.ReferenceForms.Where(c => c.RequestId == RequestID).FirstOrDefault();
            return dbresponse != null ? dbresponse.UniqueId : new Guid();

        }


        //---------------
        //Questions CRUD
        //---------------

        public List<Question> GetAllQuestions()
        {
            return _context.Questions.Where(q => q.IsActive == true).ToList();
        }

        public List<Question> GetQuestionsByText(string text)
        {
            return _context.Questions.Where(q => EF.Functions.Like(q.QuestionText!, text + "%")).ToList();
        }

        public Question UpdateQuestion(Question question)
        {
            _context.Questions.Update(question);
            _context.SaveChanges();
            return question;
        }

        public Question CreateQuestion(Question question)
        {
            _context.Questions.Add(question);
            _context.SaveChanges();
            return question;
        }

        public bool DeleteQuestion(int id)
        {
            _context.Remove(id);
            return true;
        }

        public Referee GetRefereeInfo(int refereeId)
        {
            return _context.Referees.Single(r => r.Refereeid == refereeId);
        }

        public bool SaveRefereeAnswers(List<LRefereeQAModel> lRefereeQAModel)
        {
            try
            {
                RefereeAnswer refereeAnswer;
                int CheckRequestIDInDB = 0;
                bool checkReqID = false;
                DateTime questionCreationDate = DateTime.Now;

                //Check if request already exists
                foreach (var refereeAnswers in lRefereeQAModel)
                {
                    CheckRequestIDInDB = refereeAnswers.RequestID;
                    int isRequestValid = _context.RefereeAnswers.Where(a => a.RequestId == CheckRequestIDInDB).Count();
                    if (isRequestValid > 0)
                    {
                        //get qustion creation date
                        checkReqID = true;
                        break;
                    }
                    else
                    {
                        checkReqID = false;
                    }

                }



                //Get request date 
                if (!checkReqID)
                {

                    var thisReferenceForm = _context.ReferenceForms.Where(a => a.RequestId == CheckRequestIDInDB);
                    foreach (var thisReference in thisReferenceForm)
                    {
                        questionCreationDate = thisReference.CreationDate;
                        break;
                    }
                }

                foreach (var refereeAnswers in lRefereeQAModel)
                {
                    CheckRequestIDInDB = refereeAnswers.RequestID;

                    refereeAnswer = new RefereeAnswer
                    {
                        RequestId = refereeAnswers.RequestID,
                        QuestionDate = questionCreationDate,
                        AnswerDate = refereeAnswers.CreationDate,
                        CandidateName = refereeAnswers.CandidateName,
                        RequestKey = refereeAnswers.RequestKey,
                        RefereeName = refereeAnswers.RefereeName,
                        RefereeFirstName = refereeAnswers.RefereeFirstName,
                        RefereeLastName = refereeAnswers.RefereeLastName,
                        RefereeEmail = refereeAnswers.RefereeEmail,
                        RefereeCountry = refereeAnswers.RefereeCountry,
                        RefereeMobile = refereeAnswers.RefereeMobile,
                        RefereeCompanyName = refereeAnswers.RefereeCompanyName,
                        RefereeJobTitle = refereeAnswers.RefereeJobTitle,
                        RefereeSpeciality = refereeAnswers.RefereeSpeciality,
                        ReferenceType = refereeAnswers.ReferenceType,
                        QuestionnaireSetId = refereeAnswers.QuestionnaireSetId,
                        QuestionId = refereeAnswers.QuestionID,
                        QuestionnaireSetName = refereeAnswers.QuestionnaireSetName,
                        Question = refereeAnswers.Question,
                        AnswerType = refereeAnswers.AnswerType,
                        Answer = refereeAnswers.Answer,
                        RefereeId = refereeAnswers.RefereeID,
                        CandidateFirstName = refereeAnswers.CandidateFirstName,
                        CandidateLastName = refereeAnswers.CandidateLastName,
                        AnswerWeight = refereeAnswers.AnswerWeight


                    };

                    if (!checkReqID)
                    {
                        _context.RefereeAnswers.Add(refereeAnswer);
                        _context.SaveChanges(true);
                    }

                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public int? GetAnswergroupIDByQID(int questionID)
        {
            var response = _context.Questions.Single(a => a.QuestionId == questionID);
            return response.AnswerGroupId;

        }


        //Requests
        public List<LRequestCandidateModel> GetAllRequests()
        {

            DateTime now = DateTime.Now;
            DateTime past3days = DateTime.Now.AddDays(-3);


            List<LRequestCandidateModel> retListOfRequests = new List<LRequestCandidateModel>();
            var listOfRequests = _context.Requests.Where(c => c.RequestDate <= now && c.RequestDate >= past3days).OrderByDescending(c => c.RequestDate).Take(20);

            if (listOfRequests != null)
            {
                foreach (var request in listOfRequests)
                {
                    if (request.RemoteKey != null)
                    {
                        retListOfRequests.Add(new LRequestCandidateModel
                        {
                            RequestID = request.RequestId,
                            RefereeID = request.RefereeId,
                            RemoteKey = request.RemoteKey.ToString(),
                            QuestionSetID = request.QuestionSetId,
                            Status = request.Status!.ToString(),
                            ModifiedDate = request.ModifiedDate,
                            RequestDate = request.RequestDate,
                            UserID = request.UserId

                        });
                    }

                }
            }
            return retListOfRequests;

        }

        public int GetCandidateCount()
        {
            return _context.Requests.Select(d => d.RemoteKey).Distinct().Count();
        }

        public int GetNumberOfReferences()
        {
            return _context.Requests.Select(r => r.RequestId).Count();
        }

        public int GetNumberOfCompletedReferences()
        {
            return _context.Requests.Where(c => c.Status == "Completed").Count();
        }

        public int GetCandidateScore(int RequestID)
        {
            int scoreCount;
            int scoreSum;

            if (_context.RefereeAnswers.Any(r => r.RequestId == RequestID))
            {
                // scoreCount = _context.RefereeAnswers.Where(c => c.Answer != null && c.AnswerType == "RatingInput" && c.RequestId == RequestID).Count();
                // scoreSum = _context.RefereeAnswers.Where(c => c.AnswerType == "RatingInput").Where(d => d.RequestId == RequestID).Select(i => int.Parse(i.Answer)).Sum();
                scoreSum = 0;
                scoreCount = 5;

            }
            else
            {
                scoreSum = 0;
                scoreCount = 5;

            }
            int average = scoreSum / scoreCount;
            return (average / scoreCount) * 100;

        }

        public int GetCandidatesTotalReferences(string splitCandidateID)
        {
            string newSplitCR = "'%" + splitCandidateID + "%'";

            return _context.Requests.Where(c => c.RemoteKey!.Contains(splitCandidateID)).Count();

        }

        public string GetReferenceLinkByRequestID(int RequestID)
        {
            string refLinkString = string.Empty;
            var reflink = _context.ReferenceForms.Where(c => c.RequestId == RequestID).FirstOrDefault();
            if (reflink != null)
            {
                refLinkString = reflink.Link;
            }
            return refLinkString;
        }

        public Task<List<Request>> GetUpcomingRequests()
        {
            var currentDate = DateTime.Now;
            return _context.Requests.AsNoTracking().Where(r => r.RequestDate > currentDate && r.Status == "Pending").ToListAsync();
        }

        public string GetRefereePhoneNumber(int refereeID)
        {
            var referees = _context.Referees.Single(r => r.Refereeid == refereeID);
            return referees.PhoneNumber!;

        }

        public bool DoesRefereeIDExists(int refereeId)
        {
            if (_context.Referees.Any(o => o.Refereeid == refereeId))
            {
                return true;
            }
            return false;
        }

        public void SaveNotification(string recipient, string type, string status, string message, int refereeId)
        {
            try
            {

                int refereeID = refereeId;

                if (!_context.Notifications.Any(n => n.RefereeId == refereeID && n.NotificationType == type))
                {
                    var notification = new Notification
                    {
                        RefereeId = refereeID,
                        NotificationType = type,
                        NotificationStatus = status,
                        NotificationDateTime = DateTime.Now,
                        Message = message
                    };

                    _context.Notifications.Add(notification);
                    _context.SaveChangesAsync();
                }


            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving notification: {ex.Message}");
            }
        }

        public int GetRefereeIdByRecipient(string recipient)
        {
            var referee = _context.Referees
               .AsNoTracking()
               .FirstOrDefaultAsync(r => r.PhoneNumber == recipient || r.Email == recipient);


            if (referee.Result!.Refereeid > 0)
            {
                return referee.Result.Refereeid;
            }

            return 0;
        }

        public bool UpdateReferenceForm(LNoConsentModel referenceForm)
        {
            var result = _context.ReferenceForms.SingleOrDefault(b => b.UniqueId == referenceForm.formId);
            if (result != null)
            {
                result.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool IsFormStillValid(Guid targetGuid)
        {
            var response = _context.ReferenceForms.SingleOrDefault(c => c.UniqueId == targetGuid);
            if (response != null)
            {
                if (response.IsActive)
                    return true;
                return false;
            }
            return false;

        }

        public int GetRequestIDFromRequestKey(string remoteKey)
        {
            var response = _context.Requests.FirstOrDefault(c => c.RemoteKey == remoteKey);
            return Convert.ToInt32(response!.RequestId);
        }

        public async Task<bool> RequestStatusUpdate(int requestID, string status)
        {

            if (status == null)
                return false;

            var request = _context.Requests.First(a => a.Equals(requestID));
            request.Status = status;
            var updated = await _context.SaveChangesAsync();

            return updated > 0;

        }

        public List<LRefereeEditQAModel> GetReferenceAnswersByReqKey(string RequestKey)
        {

            List<LRefereeEditQAModel> QAs = new List<LRefereeEditQAModel>();

            var referenceAnswers = _context.RefereeAnswers.Where(a => a.RequestKey == RequestKey);
            foreach (var referenceAnswer in referenceAnswers)
            {

                QAs.Add(
                    new LRefereeEditQAModel
                    {

                        Answer = referenceAnswer.Answer,
                        AnswerType = referenceAnswer.AnswerType,
                        AnswerWeight = referenceAnswer.AnswerWeight,
                        AnswerDate = referenceAnswer.AnswerDate,
                        CandidateName = referenceAnswer.CandidateName,
                        CandidateFirstName = referenceAnswer.CandidateFirstName,
                        CandidateLastName = referenceAnswer.CandidateLastName,
                        RequestID = referenceAnswer.RequestId,
                        RequestKey = referenceAnswer.RequestKey!,
                        RefereeID = referenceAnswer.RefereeId,
                        RefereeName = referenceAnswer.RefereeName,
                        RefereeFirstName = referenceAnswer.RefereeFirstName,
                        RefereeLastName = referenceAnswer.RefereeLastName,
                        RefereeEmail = referenceAnswer.RefereeEmail,
                        RefereeCompanyName = referenceAnswer.RefereeCompanyName,
                        RefereeCountry = referenceAnswer.RefereeCountry,
                        RefereeMobile = referenceAnswer.RefereeMobile,
                        RefereeJobTitle = referenceAnswer.RefereeJobTitle,
                        RefereeSpeciality = referenceAnswer.RefereeSpeciality,
                        ReferenceType = referenceAnswer.ReferenceType,
                        QuestionnaireSetId = referenceAnswer.QuestionnaireSetId,
                        QuestionnaireSetName = referenceAnswer.QuestionnaireSetName,
                        QuestionID = referenceAnswer.QuestionId,
                        Question = referenceAnswer.Question,
                        QuestionDate = referenceAnswer.QuestionDate,


                    }
                    );

            }

            return QAs;

        }

        public string GetRequestStatus(string RemoteKey)
        {
            string reqStatus = string.Empty;
            var reqData = _context.Requests.Where(a => a.RemoteKey == RemoteKey);
            foreach (var a in reqData)
            {
                reqStatus = a.Status!;
            }

            return (reqStatus);
        }

        public bool SaveEditedRefereeAnswers(List<EditDBModel> updatedAnswers)
        {
            bool saveStatus = false;
            foreach (var compValue in updatedAnswers)
            {
                saveStatus = false;
                if(compValue.QuestionID.Contains("'"))
                {

                    compValue.QuestionID = compValue.QuestionID.Trim(new Char[] { '\''});
                  
                }


                int questionId = Convert.ToInt16(compValue.QuestionID);
                var recordToUpdate = _context.RefereeAnswers.First(c => c.QuestionId == questionId && c.RequestKey == compValue.RequestKey);
                recordToUpdate.Answer = compValue.Answer;
                _context.SaveChanges();
                Thread.Sleep(30);
                saveStatus = true;
            }


            return saveStatus;
        }

        public bool UpdateRequestStatus(string RemoteKey, string Status)
        {
            bool saveStatus = false;


            var recordToUpdate = _context.Requests.First(c => c.RemoteKey == RemoteKey);
            recordToUpdate.Status = Status;

            _context.SaveChanges();
            saveStatus = true;

            return saveStatus;

        }

        public List<AnswerOptionsModel> GetAnswerOptions(int questionSetId)
        {

                  

            List<AnswerOptionsModel> _options = new List<AnswerOptionsModel>();
            var answerOptions = _context.Questions.Where(a => a.QuestionSetId == questionSetId &&  a.AnswerGroupId != null).ToList();
            
            foreach( var answerOption in answerOptions )

            {
                var options = _context.AnswerOptions.Where(b => b.AnswerGroupId == answerOption.AnswerGroupId).ToList();
                foreach (var option in options)
                {

                    if (option.Active == true)
                    {
                        _options.Add(
                                        new AnswerOptionsModel
                                        {
                                            QuestionID = answerOption.QuestionId,
                                            AnswerOptions = option.Value
                                        }
                                    );
                    }
                }

            }
            return _options;
        }

        public ReferenceExtendedData GetReferenceExtendedData(int requestId)
        {
            var result = _context.ReferenceExtendedData
                   .FromSqlRaw("EXEC ERADB.dbo.usp_GetExtendedReferenceData @RequestID",
                       new SqlParameter("@RequestID", requestId))
                   .ToList();

            if (result == null)
                throw new Exception("Record not found for Request key " + requestId);

            if (result.Count == 0)
                throw new Exception("Record not found for Request key " + requestId);

            return result[0];
        }
    }
}
