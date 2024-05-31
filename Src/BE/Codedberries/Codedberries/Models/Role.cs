using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool CanAddNewUser { get; set; }

        [Required]
        public bool CanAddUserToProject { get; set; }

        [Required]
        public bool CanRemoveUserFromProject { get; set; }

        [Required]
        public bool CanCreateProject { get; set; }

        [Required]
        public bool CanDeleteProject { get; set; }

        [Required]
        public bool CanEditProject { get; set; }

        [Required]
        public bool CanViewProject { get; set; }

        [Required]
        public bool CanAddTaskToUser { get; set; }

        [Required]
        public bool CanCreateTask { get; set; }

        [Required]
        public bool CanRemoveTask { get; set; }

        [Required]
        public bool CanEditTask { get; set; }
        
        [Required]
        public bool CanEditUser { get; set; }

        public Role(string name)
        {
            Name = name;
        }
    }
}
