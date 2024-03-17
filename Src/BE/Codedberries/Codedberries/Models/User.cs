using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Codedberries.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }
        
        [Required]
        public bool Activated { get; set; }

        public string? ProfilePicture { get; set; }

        [Required]
        public byte[] PasswordSalt {get; set;}
        
        [AllowNull]
        public string? ActivationToken { get; set; }

        [ForeignKey("RoleId")]
        public int? RoleId { get; set; }

        public ICollection<Project> Projects { get; } = new List<Project>();




        public void SetPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }


        public User(string email, string password, string firstname, string lastname, int? roleId)
        {
            Email = email;
            SetPassword(password);
            //Password = password;
            Firstname = firstname;
            Lastname = lastname;
            RoleId = roleId;
            Activated = false;
            PasswordSalt = new byte[1];
            ActivationToken = null;

            
        }
    }
}
