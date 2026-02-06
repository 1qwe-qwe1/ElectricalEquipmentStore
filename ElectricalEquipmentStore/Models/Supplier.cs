using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("suppliers")]
    public class Supplier
    {
        [Key]
        [Column("supplierid")]
        public long SupplierId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Название поставщика обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        [Column("phone")]
        [Required(ErrorMessage = "Телефон поставщика обязателен")]
        [StringLength(14, ErrorMessage = "Телефон не должен превышать 14 символов")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; } = null!;

        [Column("email")]
        [Required(ErrorMessage = "Email поставщика обязателен")]
        [StringLength(50, ErrorMessage = "Email не должен превышать 50 символов")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = null!;

        [Column("bankdetails")]
        [StringLength(50, ErrorMessage = "Банковские реквизиты не должны превышать 50 символов")]
        public string? BankDetails { get; set; }

        [Column("isactive")]
        [Required(ErrorMessage = "Статус активности обязателен")]
        public bool IsActive { get; set; }

        public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
    }
}
