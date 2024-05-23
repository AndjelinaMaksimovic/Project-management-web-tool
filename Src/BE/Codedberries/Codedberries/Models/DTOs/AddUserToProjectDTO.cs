namespace Codedberries.Models.DTOs
{
    public class AddUserToProjectDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int RoleId { get; set; }
    }
}
