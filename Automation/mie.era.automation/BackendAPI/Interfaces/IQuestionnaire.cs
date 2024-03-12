using BackendAPI.Database;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace BackendAPI.Interfaces
{
    public interface IQuestionnaire
    {

        //Task<bool> CreateReferenceCheck(ReferenceCheckRequest request);

        Task<List<AnswerType>> GetAllAnswerTypes();

        List<Models.LQuestionsModel> GetQuestionsFormBySetID(LCreateFormRequestModel LCFRequestModel);

       string GetReferenceLink(byte[] data, int RequestID);

        string CreateReferenceLink(Guid? uniqureRef);

        int CreateRequest(LRequestModel reqModel);

        //Reference Check
        byte[] ReferenceCheck(Guid? guid);
        Guid? GetReferenceFormGUUID(int RequestID);


        //---------
        //Questions CRUD
        List<Question> GetAllQuestions();
        Question GetQuestionById (int questionID);
        List<Question> GetQuestionsBySetID(int QuestionSetID);
        List<Question> GetQuestionsByText(string text);
        Question UpdateQuestion(Question question);
        Question CreateQuestion(Question question);
        bool DeleteQuestion(int id);


        //----------
        //No Consent Form
        //----------------
        string NoConsentToReference(LNoConsentModel noCModel);

        bool IsFormStillValid(Guid teragetGuid);

        string GetAManuallyFillForm(string requestKey);

    }
}
