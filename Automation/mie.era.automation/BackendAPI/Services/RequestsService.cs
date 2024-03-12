using BackendAPI.Interfaces;

namespace BackendAPI.Services
{
    public class RequestsService : IRequests
    {
        private readonly IDatabaseRepo _dbserve;

        public RequestsService(IDatabaseRepo dbserve)
        {
            _dbserve = dbserve; 
        }


        public string GetRequestStatus(string RequestKey)
        {
           return _dbserve.GetRequestStatus(RequestKey);
        }

        public bool UpdateRequestStatus(string RequestKey, string Status)
        {
            return _dbserve.UpdateRequestStatus(RequestKey,Status);
        }
    }
}
