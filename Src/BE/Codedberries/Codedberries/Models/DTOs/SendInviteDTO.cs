namespace Codedberries.Models.DTOs
{
    public class SendInviteDTO
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Email { get; set; }
        public required int RoleId { get; set; }
    }
}
