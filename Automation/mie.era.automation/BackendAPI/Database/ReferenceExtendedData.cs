namespace BackendAPI.Database
{
    public class ReferenceExtendedData
    {
        public int RequestId { get; set; }
        public string  RemoteKey { get; set; }

        public  string Country { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string ReferenceType { get; set; }
    }
}
