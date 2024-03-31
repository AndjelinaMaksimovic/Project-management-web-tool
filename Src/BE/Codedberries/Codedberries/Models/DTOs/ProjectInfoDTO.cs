namespace Codedberries.Models.DTOs
{
    public class ProjectInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool Starred { get; set; }
        public DateTime StartDate { get; set; }
    }
}
