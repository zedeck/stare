namespace BackendAPI.Models
{
    public class LQuestionsModel
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string AnswerType { get; set; } = string.Empty;
    }
}
