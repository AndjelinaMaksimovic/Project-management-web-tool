namespace Codedberries.Models.DTOs
{
    public class TaskDependencyRequestDTO
    {
        public int TaskId1 { get; set; }
        public int TaskId2 { get; set; }
        public int TypeOfDependencyId { get; set; }
    }
}
