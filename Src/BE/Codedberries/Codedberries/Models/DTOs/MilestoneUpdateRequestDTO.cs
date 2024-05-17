namespace Codedberries.Models.DTOs
{
    public class MilestoneUpdateRequestDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Date { get; set; }

        public bool IsEmpty() 
        {
            return string.IsNullOrEmpty(Name) && Date==null;
        }
    }
}
