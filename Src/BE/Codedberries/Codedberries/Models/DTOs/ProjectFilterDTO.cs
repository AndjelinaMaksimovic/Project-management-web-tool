namespace Codedberries.Models.DTOs
{
    public class ProjectFilterDTO
    {
        public int? ProjectId { get; set; }
        public List<int> AssignedTo { get; set; }
        public DateTime? StartDateAfter { get; set; }
        public DateTime? StartDateBefore { get; set; }
        public DateTime? ExactStartDate { get; set; }
        public DateTime? DueDateAfter { get; set; }
        public DateTime? DueDateBefore { get; set; }
        public DateTime? ExactDueDate { get; set; }
        public bool? IsArchived { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
    }
}
