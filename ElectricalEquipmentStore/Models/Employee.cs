using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("employeeid")]
        public long EmployeeId { get; set; }

        [Column("postid")]
        [ForeignKey("Post")]
        public long PostId { get; set; }

        [Column("phonenumber")]
        [Required(ErrorMessage = "Номер телефона обязателен")]
        [StringLength(14, ErrorMessage = "Номер телефона не должен превышать 14 символов")]
        [Phone(ErrorMessage = "Некорректный формат номера телефона")]
        public string PhoneNumber { get; set; } = null!;

        [Column("hiredate")]
        [Required(ErrorMessage = "Дата приема обязательна")]
        public DateTime HireDate { get; set; }

        [Column("userid")]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Column("salary")]
        [Required(ErrorMessage = "Зарплата обязательна")]
        [Range(0, 1000000, ErrorMessage = "Зарплата должна быть от 0 до 1,000,000")]
        public decimal Salary { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
