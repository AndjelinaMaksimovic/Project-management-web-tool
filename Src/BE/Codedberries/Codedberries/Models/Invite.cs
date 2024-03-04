namespace Codedberries.Models
{
    public class Invite
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public int RoleId { get; set; }
    }
}
