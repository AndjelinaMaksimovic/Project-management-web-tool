namespace Codedberries.Models.DTOs
{
    public class UserProjectsDTO
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
