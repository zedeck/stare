using BackendAPI.Models;

namespace BackendAPI.Interfaces
{
    public interface ICandidate
    {
        LCandidateModel GetCandidateInfo(string RemoteKey);

        LCandidateModel GetCandidateInfoByReqID(int RequestID);

        int GetCandidateCount();
        int GetTotalNumberOfReferences();
        int GetNumberOfCompletedReferences();
        List<LCandidateListModel> GetListOfCandidates();
        string GetReferenceLinkByRequestID(int RequestID);
    }
}
