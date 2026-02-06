using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("carts")]
    public class Cart
    {
        [Key]
        [Column("cartid")]
        public long CartId { get; set; }

        [Column("clientid")]
        [ForeignKey("Client")]
        public long ClientId { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
    }
}
