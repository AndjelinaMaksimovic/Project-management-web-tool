using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codedberries.Models
{
    [Table("Activities")]
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public String ActivityDescription { get; set; }

        public DateTime Time {  get; set; }

        public Activity()
        {
        }

        public Activity(int userId, int projectId, string description, DateTime time) 
        {
            UserId = userId;
            ProjectId = projectId;
            ActivityDescription = description;
            Time = time;
        }
    }
}
