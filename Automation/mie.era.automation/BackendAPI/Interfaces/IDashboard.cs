using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BackendAPI.Interfaces
{
    public interface IDashboard
    {
        bool PostResponse([FromForm] Object clientAnswer);
        String GetQustionSetInfo(int id);
        String GetQuestionById(int id);
        bool SaveRefereeAnswers(List<LRefereeQAModel> lRefereeQAModel);
        int? GetAnswergroupIDByQID(int keyVarNumber);


    }
}
