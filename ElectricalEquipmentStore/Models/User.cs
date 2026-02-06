using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Controls;

namespace ElectricalEquipmentStore.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("userid")]
        public long UserId { get; set; }

        [Column("login")]
        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов")]
        public string Login { get; set; } = null!;

        [Column("name")]
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        [Column("surname")]
        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string Surname { get; set; } = null!;

        [Column("patronymic")]
        [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов")]
        public string? Patronymic { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "Email обязателен")]
        [StringLength(50, ErrorMessage = "Email не должен превышать 50 символов")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = null!;

        [Column("passwordhash")]
        [Required(ErrorMessage = "Хэш пароля обязателен")]
        [StringLength(150, ErrorMessage = "Хэш пароля не должен превышать 150 символов")]
        public string PasswordHash { get; set; } = null!;

        [Column("roleid")]
        [ForeignKey("Role")]
        public long RoleId { get; set; }

        [Column("isactive")]
        public bool IsActive { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Column("image")]
        [StringLength(50, ErrorMessage = "Путь к изображению не должен превышать 50 символов")]
        public string? Image { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Client? Client { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
