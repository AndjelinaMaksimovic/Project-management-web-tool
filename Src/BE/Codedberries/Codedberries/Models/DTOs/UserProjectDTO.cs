namespace Codedberries.Models.DTOs
{
    public class UserProjectDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ProjectInformationDTO Project { get; set; }
    }
}
