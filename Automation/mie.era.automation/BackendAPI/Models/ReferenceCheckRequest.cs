namespace BackendAPI.Models
{
    public class ReferenceCheckRequest
    {
        public int RequestID { get; set; }
        public DateTime RequestDate { get; set; }
        public LCandidateModel Candidate { get; set; } = new LCandidateModel();
        public LRefereeModel Referee { get; set; } = new LRefereeModel();
        public ReferenceTypeModel ReferenceType { get; set; } = new ReferenceTypeModel();
        public QuestionSetType QuestionSet { get; set; } = new QuestionSetType();
    }
}
