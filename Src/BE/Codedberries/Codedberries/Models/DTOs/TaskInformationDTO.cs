namespace Codedberries.Models.DTOs
{
    public class TaskInformationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int StatusId { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int DifficultyLevel { get; set; }
        public bool Archived { get; set; }
    }
}
