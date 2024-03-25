using Codedberries.Common;

namespace Codedberries.Models.DTOs
{
    public class TaskCreationRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
       
        public string Status { get; set; }
        public string Priority { get; set; }
        public int DifficultyLevel { get; set; }
        public int CategoryId { get; set; }
        public List<int> DependencyIds { get; set; }
        public int ProjectId { get; set; }
    }
}
