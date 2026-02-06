using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("paymentmethods")]
    public class PaymentMethod
    {
        [Key]
        [Column("paymentmethodid")]
        public long PaymentMethodId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название способа оплаты обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
