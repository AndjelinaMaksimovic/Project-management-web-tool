namespace Codedberries.Models
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string ActivityDescription { get; set; }
        public bool Seen {  get; set; }
        public DateTime Time { get; set; }
    }
}
