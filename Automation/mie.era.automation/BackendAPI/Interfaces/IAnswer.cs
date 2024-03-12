using BackendAPI.Models;
using Microsoft.AspNetCore.SignalR;
using System.Net.Mime;

namespace BackendAPI.Interfaces
{
    public interface IAnswer
    {
        string GetReferenceAnswers(string RequestKey);
        string CreateEditingForm(List<LRefereeEditQAModel> answers, int page);
        List<LRefereeEditQAModel> GetExistingReferenceForEdit(string id);
    }
}
