namespace BackendAPI.Models
{
    public class QuestionSetModel
    {
        public int Id { get; set; }
        public QuestionSetType QuestinSetType { get; set; } = new QuestionSetType();
        public List<QuestionsValuesModel> Questions { get; set; } = new List<QuestionsValuesModel>();
    }
}
