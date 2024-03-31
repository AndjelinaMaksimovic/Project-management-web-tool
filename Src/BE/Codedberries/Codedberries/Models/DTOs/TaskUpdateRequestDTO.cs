namespace Codedberries.Models.DTOs
{
    public class TaskUpdateRequestDTO
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DueDate { get; set; }
        public int? UserId { get; set; }
        public int? DifficultyLevel { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) &&
                   string.IsNullOrEmpty(Description) &&
                   CategoryId == null &&
                   PriorityId == null &&
                   StatusId == null &&
                   DueDate == null &&
                   UserId == null &&
                   DifficultyLevel == null;
        }
    }
}
