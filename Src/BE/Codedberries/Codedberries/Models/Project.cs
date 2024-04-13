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

        [Required]
        public string Description { get; set; }

        public ICollection<User> Users { get; } = new List<User>();
        public ICollection<Status> Statuses { get; set; }
        public ICollection<Category> Categories { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public Boolean Archived { get; set; }

        public Project(string name, string description, DateTime dueDate)
        {
            Name = name;
            Description = description;
            DueDate = dueDate;
            StartDate = DateTime.Today; 
        }
    }
}
