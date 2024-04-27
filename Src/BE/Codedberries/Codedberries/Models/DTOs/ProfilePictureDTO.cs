namespace Codedberries.Models.DTOs
{
    public class ProfilePictureDTO
    {
        public int UserId { get; set; }
        public byte[] ImageBytes { get; set; }
        public string ImageName { get; set; }
    }
}
