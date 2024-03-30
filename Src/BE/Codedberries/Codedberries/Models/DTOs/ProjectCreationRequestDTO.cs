using Codedberries.Helpers;

namespace Codedberries.Models.DTOs
{
    public class ProjectCreationRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public List<int> UserIds { get; set; }
        public bool IsStarred { get; set; }
    }
}
