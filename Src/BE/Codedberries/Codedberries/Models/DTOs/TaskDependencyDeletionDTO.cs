namespace Codedberries.Models.DTOs
{
    public class TaskDependencyDeletionDTO
    {
        public int TaskId { get; set; }
        public int DependentTaskId { get; set; }
    }
}
