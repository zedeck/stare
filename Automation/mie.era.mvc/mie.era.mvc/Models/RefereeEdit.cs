namespace mie.era.mvc.Models
{
    public class RefereeEdit
    {
        public int RefereeId { get; set; }
        public string? Name { get; set; }
        public  string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RelationShip { get; set; }
        public bool IsActive { get; set; }
    }    
}
