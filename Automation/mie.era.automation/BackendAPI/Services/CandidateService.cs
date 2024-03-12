using BackendAPI.Interfaces;
using BackendAPI.Models;

using System.Reflection.Metadata.Ecma335;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;


namespace BackendAPI.Services
{
    public class CandidateService : ICandidate
    {
        private readonly IDatabaseRepo _dbserve;

        public CandidateService(IDatabaseRepo dbserve)
        {
            _dbserve = dbserve;
        }



        public LCandidateModel GetCandidateInfo(string RemoteKey)
        {
            return _dbserve.SP_GetCandidateInfo(RemoteKey);

        }



        public int GetNumberOfCompletedReferences()
        {
            return _dbserve.GetNumberOfCompletedReferences();
        }

        public int GetTotalNumberOfReferences()
        {
            return _dbserve.GetNumberOfReferences();
        }


        public List<LCandidateListModel> GetListOfCandidates()
        {
            List<LCandidateListModel> listOfCandidates = new List<LCandidateListModel>();
            string FullName;
            string EmailAddress;
            string MobileNumber = string.Empty;


            var listOfRequests = _dbserve.GetAllRequests();


            if (listOfRequests != null)
            {

                foreach (var request in listOfRequests)
                {
                    if (request.RemoteKey != null)
                    {


                        var candidateInfo = _dbserve.SP_GetCandidateInfo(request.RemoteKey.ToString());
                        FullName = candidateInfo.CandidateName + " " + candidateInfo.CandidateSurname;
                        EmailAddress = candidateInfo.CandidateEmail;
                        MobileNumber = candidateInfo.CandidateCell;

                        int candidateScore = _dbserve.GetCandidateScore(request.RequestID);
                        string[] splitCandidateID = request.RemoteKey.Split('-');
                        int candidatesTotalReferences = _dbserve.GetCandidatesTotalReferences(splitCandidateID[0]);
                        string AssignedTo = GetCurrentUser();

                        if (!String.IsNullOrEmpty(candidateInfo.CandidateName) && !String.IsNullOrEmpty(request.RemoteKey))
                        {

                            listOfCandidates.Add(new LCandidateListModel
                            {
                                RequestKey = request.RequestID,
                                FullName = FullName,
                                EmailAddress = EmailAddress,
                                UIMobileNumber = MobileNumber,
                                DateCreated = request.RequestDate,
                                Score = candidateScore.ToString() + "/100",
                                TotalReferences = splitCandidateID[1].ToString() + "/" + candidatesTotalReferences.ToString(),
                                ReferenceStatus = request.Status,
                                AssignedTo = AssignedTo
                            });
                        }
                    }


                }
            }
            return listOfCandidates;
        }

        public int GetCandidateCount()
        {
            return _dbserve.GetCandidateCount();
        }

        public string GetCurrentUser()
        {
            return "godwillm";    //this must call the auth api
        }

        public string GetReferenceLinkByRequestID(int RequestID)
        {
            return _dbserve.GetReferenceLinkByRequestID(RequestID);
        }

        public LCandidateModel GetCandidateInfoByReqID(int RequestID)
        {
            return _dbserve.SP_GetCandidateInfoByReqID( RequestID);
        }


    }
}
