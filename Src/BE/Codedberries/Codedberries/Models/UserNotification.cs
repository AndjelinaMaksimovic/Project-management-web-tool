using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codedberries.Models
{
    [Table("userNotification")]
    public class UserNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [Required]
        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }

        [Required]
        public bool Seen { get; set; }

        public UserNotification( int userId,int activityId,bool seen)
        {
            UserId = userId;
            ActivityId = activityId;
            Seen = seen;
        }

        public UserNotification() { }
    }
}
