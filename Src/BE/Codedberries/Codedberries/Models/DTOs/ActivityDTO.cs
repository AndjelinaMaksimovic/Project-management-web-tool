namespace Codedberries.Models.DTOs
{
    public class ActivityDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string ActivityDescription { get; set; }
    }
}
