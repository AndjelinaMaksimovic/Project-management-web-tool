namespace Codedberries.Models.DTOs
{
    public class UserProjectInformationDTO
    {
        public int ProjectId { get; set; }
        public int RoleIdOnProject { get; set; }
        public string RoleNameOnProject { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectDueDate { get; set; }
        public bool ProjectArchived { get; set; }
        public bool ProjectIsStarred { get; set; }
        public List<StatusDTO> ProjectStatuses { get; set; }
        public List<CategoryDTO> ProjectCategories { get; set; }
        public List<UserDTO> ProjectUsers { get; set; }
    }
}
