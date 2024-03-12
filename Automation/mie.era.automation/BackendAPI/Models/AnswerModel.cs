namespace BackendAPI.Models
{
    public class AnswerModel
    {
        public int Id { get; set; }
        public AnswerTypeModel AnswerType { get; set; } = new AnswerTypeModel();
        public List<string> ActualAnswers { get; set; } = new List<string>();
    }
}
