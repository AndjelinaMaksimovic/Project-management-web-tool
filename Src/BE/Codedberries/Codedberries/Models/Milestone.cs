using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    [Table("Milestones")]
    public class Milestone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MilestoneId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public Milestone(string name, int projectId, DateTime date)
        {
            Name = name;
            ProjectId = projectId;
            Date = date;
        }
    }
}