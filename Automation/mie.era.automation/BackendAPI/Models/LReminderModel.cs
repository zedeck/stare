namespace BackendAPI.Models
{
    public class LReminderModel
    {

        public string RefereeName { get; set; } = string.Empty;
        public string RefereeEmail { get; set; } = string.Empty;
        public string RefereePhone { get; set; } = string.Empty;
        public string ReferenceName { get; set; } = string.Empty;
        public int ReferenceType { get; set; }
        public bool SaveNotification { get; set; }
        public string LinkURL { get; set; } = string.Empty;
        public int RefereeId { get; set; }

    }
}
