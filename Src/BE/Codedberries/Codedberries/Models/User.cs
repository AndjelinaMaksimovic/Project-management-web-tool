using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        
        [AllowNull]
        public string? ActivationToken { get; set; }

        [ForeignKey("RoleId")]
        public int? RoleId { get; set; }

        public ICollection<Project> Projects { get; } = new List<Project>();

        public User(string email, string password, string firstname, string lastname, int? roleId)
        {
            Email = email;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            RoleId = roleId;
            Activated = false;
            ActivationToken = null;
        }
    }
}
