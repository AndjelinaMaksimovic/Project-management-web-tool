namespace Codedberries.Models.DTOs
{
    public class UpdatedProjectInfoDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UserDTO>? Users { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
