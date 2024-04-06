namespace Codedberries.Models.DTOs
{
    public class UserInformationDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool Activated { get; set; }
        public string? ProfilePicture { get; set; }
        public int? RoleId { get; set; }
        public ICollection<ProjectDTO> Projects { get; set; }
    }
}
