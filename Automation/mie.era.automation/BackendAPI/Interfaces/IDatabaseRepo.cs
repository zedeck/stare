using BackendAPI.Database;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BackendAPI.Interfaces
{
    public interface IDatabaseRepo
    {

        //---------------
        //Questions CRUD
        //---------------

        Task<List<AnswerType>> GetAllAnswerTypes();
        Task<List<AnswerOption>> GetListOfItems(int AnswerGroupID);
        Question GetQuestionById(int QuestionID);
        Int32 GetFrontEndControlTypeById(Int32 AnswerGroupID);
        QuestionSet GetQuestionSetByID(int QuestionSetID);
        string GetQuestionSetNameByID(int QuestionSetID);

        List<Question> GetAllQuestions();
        Task<IEnumerable<Question>> GetQuestionsBySetID(int QuestionSetID);
        List<Question> GetQuestionsByText(string text);
        Question UpdateQuestion(Question question);
        Question CreateQuestion(Question question);
        bool DeleteQuestion(int id);
        int? GetAnswergroupIDByQID(int questionID);

        //-----------------
        //Trailing Question
        //-----------------
        //Get Trailing QuestionText by LeadingQuestionID
        string? GetTrailingQText(int? LeadingQuestionID);
        //Get TrailingQuestionID 
        int? GetTrailingQID(string? TrailingQuestionText, int? LeadingQuestionID);
        //Get Trailing Question Answer
        string? GetTrailingQAnswer(int TrailingQuestionID);



        //---------
        //Candidate information
        //---------
        LCandidateModel SP_GetCandidateInfo(string RemoteKey);

        LCandidateModel SP_GetCandidateInfoByReqID(int RequestID);


        //---------------
        //Form Data CRUD
        //---------------
        string CreateReferenceForm(LReferenceForm referenceForm);
        byte[] GetReferenceForm(Guid? uniqueId);
        bool UpdateReferenceForm(LReferenceForm referenceForm);
        bool DeleteReferenceForm(int RequestID);

         Guid?  GetReferenceFormGUUID(int RequestID);

        //----------
        //Referee
        //----------
        Referee GetRefereeInfo(int refereeId);

        bool SaveRefereeAnswers(List<LRefereeQAModel> lRefereeQAModel);



        //Requests
        List<LRequestCandidateModel> GetAllRequests();
        int GetCandidateCount();
        int GetNumberOfReferences();
        int GetNumberOfCompletedReferences();
        int GetCandidateScore(int RequestID);
        int GetCandidatesTotalReferences(string splitCandidateID);
        string GetReferenceLinkByRequestID(int RequestID);

        public ReferenceExtendedData GetReferenceExtendedData(int requestId);
        


            //Reminder Services
            Task<List<Request>> GetUpcomingRequests();
        string GetRefereePhoneNumber(int refereeID);
        void SaveNotification(string recipient, string type, string status, string message, int refereeId);
        int GetRefereeIdByRecipient(string recipient);

        //Record Eists
        bool DoesRefereeIDExists(int refereeId);


        bool UpdateReferenceForm(LNoConsentModel referenceForm);

        bool IsFormStillValid(Guid teragetGuid);

        int GetRequestIDFromRequestKey(string requestKey);


        //-----------
        //Request status
        Task<bool> RequestStatusUpdate(int requestID, string status);
        string GetRequestStatus(string RemoteKey);

        //-----------s
        //Answers 
        List<LRefereeEditQAModel> GetReferenceAnswersByReqKey(string RequestKey);

        bool SaveEditedRefereeAnswers(List<EditDBModel> updatedAnswers);
        bool  UpdateRequestStatus(string RequestKey, string Status);
        List<AnswerOptionsModel> GetAnswerOptions(int questionSetId);

    }
}
