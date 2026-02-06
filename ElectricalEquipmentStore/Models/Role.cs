using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricalEquipmentStore.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("roleid")]
        public long RoleId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название роли обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
