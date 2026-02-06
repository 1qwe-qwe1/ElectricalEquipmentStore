using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("statusesproduct")]
    public class ProductStatus
    {
        [Key]
        [Column("statusproductid")]
        public long StatusProductId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название статуса обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
