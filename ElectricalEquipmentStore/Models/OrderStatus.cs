using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("statuses")]
    public class OrderStatus
    {
        [Key]
        [Column("statusid")]
        public long StatusId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название статуса заказа обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
