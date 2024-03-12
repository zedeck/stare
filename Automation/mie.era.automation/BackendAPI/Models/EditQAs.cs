namespace BackendAPI.Models
{
    public class EditQAs
    {
        public int QuestionID { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string AnswerType { get; set; } = string.Empty;
        public int AnswerWeight { get; set; }
        public DateTime AnswerDate { get; set; }
    }
}
