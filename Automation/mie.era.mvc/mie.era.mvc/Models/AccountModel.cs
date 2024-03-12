

namespace mie.era.mvc.Models
{
    

    public class AuthenticateResponse
    {
        public bool Success { get; set; }
        public InternalPerson? Person { get; set; }
        public string Token { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class InternalPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string DomainUserName { get; set; }
        public bool IsActive { get; set; }
        public List<InternalPersonFunction> InternalFunctions { get; set; }
    }

    public class InternalPersonFunction
    {
        public InternalPersonFunction() { }
        public int InternalPersonId { get; set; }
        public int InternalFunctionId { get; set; }
        public int InternalFunctionGroupId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionCode { get; set; }
        public string HelpText { get; set; }
        public string GroupName { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }

    }
}
