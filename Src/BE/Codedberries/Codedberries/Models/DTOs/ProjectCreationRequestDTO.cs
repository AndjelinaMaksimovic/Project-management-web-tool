using Codedberries.Helpers;

namespace Codedberries.Models.DTOs
{
    public class ProjectCreationRequestDTO
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentProjectId { get; set; }
        public List<int> UserIds { get; set; }
    }
}
