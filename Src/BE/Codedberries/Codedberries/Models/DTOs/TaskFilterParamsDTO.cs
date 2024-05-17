namespace Codedberries.Models.DTOs
{
    public class TaskFilterParamsDTO
    {
        public int ProjectId { get; set; } // required
        public int? AssignedTo { get; set; } 
        public int? StatusId { get; set; } 
        public int? PriorityId { get; set; }
        public string? TaskName {  get; set; }
        public int? DifficultyLevelGreaterThan { get; set; }
        public int? DifficultyLevelLesserThan { get; set; }
        public int? DifficultyLevelEquals { get; set; }
        public DateTime? DueDateAfter { get; set; } 
        public DateTime? DueDateBefore { get; set; } 
        public int? CategoryId { get; set; }

        public bool IsEmpty()
        {
            return ProjectId == 0 && !AssignedTo.HasValue && !StatusId.HasValue
                && !PriorityId.HasValue && !DifficultyLevelGreaterThan.HasValue &&
                !DifficultyLevelLesserThan.HasValue && !DifficultyLevelEquals.HasValue &&
                !DueDateAfter.HasValue && !DueDateBefore.HasValue && !CategoryId.HasValue && string.IsNullOrEmpty(TaskName);
        }
    }
}
