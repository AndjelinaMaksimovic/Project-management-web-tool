namespace Codedberries.Models.DTOs
{
    public class UpdatedMilestoneInfoDTO
    {
        public int MilestoneId { get; set; }
        public string Name { get; set;}
        public DateTime Date { get; set;}
        public int ProjectId { get; set;}
    }
}
