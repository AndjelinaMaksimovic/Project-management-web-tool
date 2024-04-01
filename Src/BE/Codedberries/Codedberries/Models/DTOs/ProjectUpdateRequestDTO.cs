using System.Xml.Linq;

namespace Codedberries.Models.DTOs
{
    public class ProjectUpdateRequestDTO
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public List<UserDTO>? Users { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ProjectName) &&
                   string.IsNullOrEmpty(ProjectDescription) &&
                   Users == null &&
                   DueDate == null &&
                   StartDate == null;
                   

        }
    }
}
