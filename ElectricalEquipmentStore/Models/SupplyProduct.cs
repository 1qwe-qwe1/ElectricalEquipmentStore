using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("supplyproducts")]
    public class SupplyProduct
    {
        [Key]
        [Column("supplyproductid")]
        public long SupplyProductId { get; set; }

        [Column("supplyid")]
        [ForeignKey("Supply")]
        public long SupplyId { get; set; }

        [Column("productid")]
        [ForeignKey("Product")]
        public long ProductId { get; set; }

        [Column("quantity")]
        [Required(ErrorMessage = "Количество обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
        public int Quantity { get; set; }

        public virtual Supply Supply { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
