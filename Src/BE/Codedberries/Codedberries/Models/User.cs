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
        public Role Role { get; set; }

        public ICollection<Project> Projects { get; } = new List<Project>();

        public string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
                var hashedBytes = sha256.ComputeHash(saltedPassword);
                string pass  = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return pass;
            }
        }

        public void GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            PasswordSalt = salt;
        }

        public User(string email, string password, string firstname, string lastname, int? roleId)
        {
            Email = email;
            GenerateSalt();
            string pass=HashPassword(password,PasswordSalt);
            Password = pass;
            Firstname = firstname;
            Lastname = lastname;
            RoleId = roleId;
            Activated = false;
            ActivationToken = null;
        }
    }
}
