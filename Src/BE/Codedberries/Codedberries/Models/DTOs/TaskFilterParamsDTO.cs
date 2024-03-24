namespace Codedberries.Models.DTOs
{
    public class TaskFilterParamsDTO
    {
        public int ProjectId { get; set; } // required
        public int? AssignedTo { get; set; } 
        public string Status { get; set; } 
        public string? Priority { get; set; }
        public int? DifficultyLevelGreaterThan { get; set; }
        public int? DifficultyLevelLesserThan { get; set; }
        public int? DifficultyLevelEquals { get; set; }
        public DateTime? DueDateAfter { get; set; } 
        public DateTime? DueDateBefore { get; set; } 
        public int? CategoryId { get; set; }
    }
}
