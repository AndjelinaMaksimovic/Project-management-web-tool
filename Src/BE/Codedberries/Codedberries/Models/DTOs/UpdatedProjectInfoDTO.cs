namespace Codedberries.Models.DTOs
{
    public class UpdatedProjectInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool Archived { get; set; }
        public List<StatusDTO> Statuses { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public List<UserDTO> Users { get; set; }
    }
}
