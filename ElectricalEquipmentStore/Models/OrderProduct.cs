using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("orderproducts")]
    public class OrderProduct
    {
        [Key]
        [Column("orderproductid")]
        public long OrderProductId { get; set; }

        [Column("orderid")]
        [ForeignKey("Order")]
        public long OrderId { get; set; }

        [Column("productid")]
        [ForeignKey("Product")]
        public long ProductId { get; set; }

        [Column("quantity")]
        [Required(ErrorMessage = "Количество обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
        public int Quantity { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
