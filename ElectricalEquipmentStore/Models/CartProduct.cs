using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("cartproducts")]
    public class CartProduct
    {
        [Key]
        [Column("cartproductid")]
        public long CartProductId { get; set; }

        [Column("cartid")]
        [ForeignKey("Cart")]
        public long CartId { get; set; }

        [Column("productid")]
        [ForeignKey("Product")]
        public long ProductId { get; set; }

        [Column("quantity")]
        [Required(ErrorMessage = "Количество обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
        public int Quantity { get; set; }

        public virtual Cart Cart { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
