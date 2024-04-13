namespace Codedberries.Models.DTOs
{
    public class StatusOrderChangeDTO
    {
        public int ProjectId { get; set; }
        public List<int> NewOrder { get; set; }
    }
}
