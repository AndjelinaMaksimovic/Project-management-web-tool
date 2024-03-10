using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Description { get; set; }

        public DateTime DueDate { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Required]
        public bool Status { get; set; }

        public ICollection<Task> Dependencies { get; } = new List<Task>();
    }
}
