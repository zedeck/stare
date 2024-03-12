using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;



namespace BackendAPI.Services
{
    public class DashboardService : IDashboard
    {


        private readonly IDatabaseRepo _dserve;

        public DashboardService(IDatabaseRepo dserve)
        {
            _dserve = dserve;
        }


        public bool PostResponse([FromForm] Object content)
        {
            return true;
        }


        public String GetQustionSetInfo(int  id)
        {
            
            return  _dserve.GetQuestionSetNameByID(id);
        }

        public String GetQuestionById(int id)
        {
            string respString = "No question found";
            var response = _dserve.GetQuestionById(id);
            if (response != null)
            {
                respString = response.QuestionText!.ToString();
            }
            return respString;
                        
        }

        public bool SaveRefereeAnswers(List<LRefereeQAModel> lRefereeQAModel)
        {
            return _dserve.SaveRefereeAnswers(lRefereeQAModel);
        }

        public int? GetAnswergroupIDByQID(int questionID)
        {
            return _dserve.GetAnswergroupIDByQID(questionID);
        }
    }
}
