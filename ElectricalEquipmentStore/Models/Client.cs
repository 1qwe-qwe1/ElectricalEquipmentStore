using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("clients")]
    public class Client
    {
        [Key]
        [Column("clientid")]
        public long ClientId { get; set; }

        [Column("phonenumber")]
        [StringLength(14, ErrorMessage = "Номер телефона не должен превышать 14 символов")]
        [Phone(ErrorMessage = "Некорректный формат номера телефона")]
        public string? PhoneNumber { get; set; }

        [Column("userid")]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Column("city")]
        [StringLength(30, ErrorMessage = "Город не должен превышать 30 символов")]
        public string? City { get; set; }

        [Column("street")]
        [StringLength(30, ErrorMessage = "Улица не должна превышать 30 символов")]
        public string? Street { get; set; }

        [Column("building")]
        [StringLength(5, ErrorMessage = "Номер дома не должен превышать 5 символов")]
        public string? Building { get; set; }

        [Column("apartment")]
        [Range(1, 1000, ErrorMessage = "Номер квартиры должен быть от 1 до 1000")]
        public int? Apartment { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
