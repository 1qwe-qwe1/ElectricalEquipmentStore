using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("categoryid")]
        public long CategoryId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        [Column("description")]
        [StringLength(250, ErrorMessage = "Описание не должно превышать 250 символов")]
        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
