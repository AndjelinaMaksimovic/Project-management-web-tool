using System.Xml.Linq;

namespace Codedberries.Models.DTOs
{
    public class ProjectUpdateRequestDTO
    {
        public int ProjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<int>? Users { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) &&
                   string.IsNullOrEmpty(Description) &&
                   Users == null &&
                   DueDate == null &&
                   StartDate == null;
                   

        }
    }
}
