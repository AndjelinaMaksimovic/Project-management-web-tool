namespace Codedberries.Models.DTOs
{
    public class TaskDependencyRequestDTO
    {
        public int TaskId { get; set; } 
        public int DependentTaskId { get; set; } 
        public int TypeOfDependencyId { get; set; }
    }
}
