using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("posts")]
    public class Post
    {
        [Key]
        [Column("postid")]
        public long PostId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название должности обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        // Навигационные свойства
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
