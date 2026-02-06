using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("manufacturers")]
    public class Manufacturer
    {
        [Key]
        [Column("manufacturerid")]
        public long ManufacturerId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название производителя обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
