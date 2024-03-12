namespace BackendAPI.Interfaces
{
    public interface IRequests
    {
        string GetRequestStatus(string RequestKey);
        bool UpdateRequestStatus(string RequestKey, string Status);
    }
}
