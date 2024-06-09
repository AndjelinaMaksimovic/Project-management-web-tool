using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codedberries.Models
{
    [Table("PasswordChangeToken")]
    public class PasswordChangeToken
    {
        [Key]
        public string Token { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required]
        public int UserId { get; set; }

        public PasswordChangeToken(int userId)
        {
            UserId = userId;
        }
    }
}
