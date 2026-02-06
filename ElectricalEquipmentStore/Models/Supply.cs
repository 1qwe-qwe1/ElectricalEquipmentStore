using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("supplies")]
    public class Supply
    {
        [Key]
        [Column("supplyid")]
        public long SupplyId { get; set; }

        [Column("supplierid")]
        [ForeignKey("Supplier")]
        public long SupplierId { get; set; }

        [Column("orderdate")]
        [Required(ErrorMessage = "Дата заказа обязательна")]
        public DateTime OrderDate { get; set; }

        [Column("totalamount")]
        [Required(ErrorMessage = "Общая сумма обязательна")]
        [Range(0.01, 10000000, ErrorMessage = "Сумма должна быть от 0.01 до 10,000,000")]
        public decimal TotalAmount { get; set; }

        [Column("deliverydate")]
        [Required(ErrorMessage = "Дата доставки обязательна")]
        [FutureDate(ErrorMessage = "Дата доставки должна быть в будущем")]
        public DateTime DeliveryDate { get; set; }

        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<SupplyProduct> SupplyProducts { get; set; } = new List<SupplyProduct>();
    }
}
