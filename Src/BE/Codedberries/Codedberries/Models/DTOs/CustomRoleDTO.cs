namespace Codedberries.Models.DTOs
{
    public class CustomRoleDTO
    {
        public string? CustomRoleName { get; set; }
        public List<string>? Permissions { get; set; }
        public int ProjectId { get; set; }
    }
}
