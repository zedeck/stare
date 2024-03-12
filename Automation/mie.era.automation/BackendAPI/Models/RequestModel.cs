namespace BackendAPI.Models
{
    public class RequestModel
    {

        public int Id { get; set; }
        public UserModel Agent { get; set; } = new UserModel();
        public QuestionSetModel QuestionSet { get; set; } = new QuestionSetModel(); //QuestionSet ID
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        //public string Description { get; set; } = string.Empty;
        public int RemoteKey { get; set; }
    }
}
