using Codedberries.Common;

namespace Codedberries.Models.DTOs
{
    public class TaskCreationRequestDTO
    {
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public StatusEnum Status { get; set; }
        public PriorityEnum Priority { get; set; }
        public int DifficultyLevel { get; set; }
        public int CategoryId { get; set; }
        public List<int> DependencyIds { get; set; }
    }
}
