namespace Codedberries.Models.DTOs
{
    public class MilestoneCreationRequestDTO
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int ProjectId { get; set; }
    }
}
