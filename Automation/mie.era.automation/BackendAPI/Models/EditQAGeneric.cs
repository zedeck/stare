namespace BackendAPI.Models
{
    public class EditQAGeneric
    {
        public string RequestKey { get; set; }
        public Int32 RequestID { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public DateTime QuestionDate { get; set; }
        public int QuestionnaireSetId { get; set; }
        public string QuestionnaireSetName { get; set; } = string.Empty;
       

    }
}
