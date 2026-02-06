using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("paymentstatuses")]
    public class PaymentStatus
    {
        [Key]
        [Column("paymentstatusid")]
        public long PaymentStatusId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название статуса оплаты обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
