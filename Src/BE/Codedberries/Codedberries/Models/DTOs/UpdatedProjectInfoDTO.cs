namespace Codedberries.Models.DTOs
{
    public class UpdatedProjectInfoDTO
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public List<UserDTO>? Users { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
