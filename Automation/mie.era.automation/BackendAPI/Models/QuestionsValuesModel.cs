namespace BackendAPI.Models
{
    public class QuestionsValuesModel
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public AnswerModel Answer { get; set; } = new AnswerModel();
    }
}
