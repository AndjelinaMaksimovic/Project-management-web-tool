namespace Codedberries.Models.DTOs
{
    public class ProjectTasksInfoDTO
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public List<TaskUserInfoDTO> AssignedTo { get; set; }
    }
}
