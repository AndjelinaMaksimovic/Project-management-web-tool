namespace Codedberries.Models.DTOs
{
    public class UpdatePasswordRequestDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
