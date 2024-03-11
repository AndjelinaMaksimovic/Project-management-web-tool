using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    [Table("Permissions")]
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Description { get; set; }

        public ICollection<Role> Roles { get; } = new List<Role>();

        public Permission(string? description)
        {
            Description = description;
        }
    }
}
