namespace BackendAPI.Models
{
    public class LEmailModel
    {
        public int priority { get; set; }
        public int encoding { get; set; }

        public LEmailFromModel from { get; set; } = new LEmailFromModel();
        public LEmailReplyToModel replyTo { get; set; } = new LEmailReplyToModel();
        public LEmailToModel to { get; set; } = new LEmailToModel();
        public LEmailAttachmentsModel attachments { get; set; } = new LEmailAttachmentsModel();
        public LEmailCCModel cc { get; set; } = new LEmailCCModel();
        public LEmailBCCModel bcc { get; set; } = new LEmailBCCModel();

        public string subject { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public bool readReceipt { get; set; }
        public bool deliveryReceipt { get; set; }

    }
}
