namespace Codedberries.Models.DTOs
{
    public class RolePermissionDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool CanAddNewUser { get; set; }
        public bool CanAddUserToProject { get; set; }
        public bool CanRemoveUserFromProject { get; set; }
        public bool CanCreateProject { get; set; }
        public bool CanDeleteProject { get; set; }
        public bool CanEditProject { get; set; }
        public bool CanViewProject { get; set; }
        public bool CanAddTaskToUser { get; set; }
        public bool CanCreateTask { get; set; }
        public bool CanRemoveTask { get; set; }
        public bool CanEditTask { get; set; }
        public bool CanEditUser { get; set;}
    }
}
