using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codedberries.Models
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<User> Users { get; } = new List<User>();

        [ForeignKey("ProjectId")]
        public int? ParentProjectId { get; set; }

        public Project(string name, int? parentProjectId)
        {
            Name = name;
            ParentProjectId = parentProjectId;
        }
    }
}
