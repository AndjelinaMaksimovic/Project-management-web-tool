namespace Codedberries.Models.DTOs
{
    public class ProjectTasksInfoDTO
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; } 
        public int? PriorityId { get; set; } 
        public int? StatusId { get; set; } 
        public string CategoryName { get; set; } 
        public string PriorityName { get; set; } 
        public string StatusName { get; set; } 
        public DateTime StartDate {  get; set; }
        public DateTime DueDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public int DifficultyLevel { get; set; }
        public List<TaskUserInfoDTO> AssignedTo { get; set; }
        public List<int> DependentTasks { get; set; }
    }
}
