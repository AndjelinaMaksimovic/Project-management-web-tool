using Codedberries.Common;

namespace Codedberries.Models.DTOs
{
    public class TaskCreationRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }      
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int DifficultyLevel { get; set; }
        public int CategoryId { get; set; }
        public List<int> DependencyIds { get; set; }
        public List<int> TypeOfDependencyIds { get; set; }
        public int ProjectId { get; set; }
        public List<int> UserIds { get; set; }
    }
}
