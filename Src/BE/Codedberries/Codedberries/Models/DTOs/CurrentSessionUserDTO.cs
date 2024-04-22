namespace Codedberries.Models.DTOs
{
    public class CurrentSessionUserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool Activated { get; set; }
        public string? ProfilePicture { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<ProjectInformationDTO>? Projects { get; set; }
        public List<TaskInformationDTO>? Tasks { get; set; }
    }
}
