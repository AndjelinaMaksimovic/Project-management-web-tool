using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codedberries.Models
{
    public class RolePermission
    {
        [Key]
        [Column(Order = 1)]
        public int RoleId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int PermissionId { get; set; }
    }
}
