using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("deliverymethods")]
    public class DeliveryMethod
    {
        [Key]
        [Column("deliverymethodid")]
        public long DeliveryMethodId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название способа доставки обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
