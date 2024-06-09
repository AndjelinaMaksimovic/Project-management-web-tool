namespace Codedberries.Models.DTOs
{
    public class TaskUpdateRequestDTO
    {
        public int TaskId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? FirstTaskDependency { get; set; }
        public int? SecondTaskDependency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public List<int>? UserIds { get; set; }
        public int? DifficultyLevel { get; set; }
        public int? ProjectId { get; set; }
        public bool? ForceDateChange { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) &&
                   string.IsNullOrEmpty(Description) &&
                   CategoryId == null &&
                   PriorityId == null &&
                   StatusId == null &&
                   DueDate == null &&
                   StartDate == null &&
                   UserIds == null &&
                   DifficultyLevel == null &&
                   ProjectId == null &&
                   FirstTaskDependency == null &&
                   SecondTaskDependency == null;
        }
    }
}
