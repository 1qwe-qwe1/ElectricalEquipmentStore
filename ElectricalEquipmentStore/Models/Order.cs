using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("orderid")]
        public long OrderId { get; set; }

        [Column("clientid")]
        [ForeignKey("Client")]
        public long ClientId { get; set; }

        [Column("totalamount")]
        [Required(ErrorMessage = "Общая сумма обязательна")]
        [Range(0.01, 10000000, ErrorMessage = "Сумма должна быть от 0.01 до 10,000,000")]
        public decimal TotalAmount { get; set; }

        [Column("statusid")]
        [ForeignKey("Status")]
        public long StatusId { get; set; }

        [Column("paymentmethodid")]
        [ForeignKey("PaymentMethod")]
        public long PaymentMethodId { get; set; }

        [Column("paymentstatusid")]
        [ForeignKey("PaymentStatus")]
        public long PaymentStatusId { get; set; }

        [Column("deliverymethodid")]
        [ForeignKey("DeliveryMethod")]
        public long DeliveryMethodId { get; set; }

        [Column("deliverycity")]
        [Required(ErrorMessage = "Город доставки обязателен")]
        [StringLength(50, ErrorMessage = "Город не должен превышать 50 символов")]
        public string DeliveryCity { get; set; } = null!;

        [Column("deliverystreet")]
        [Required(ErrorMessage = "Улица доставки обязательна")]
        [StringLength(50, ErrorMessage = "Улица не должна превышать 50 символов")]
        public string DeliveryStreet { get; set; } = null!;

        [Column("deliverybuilding")]
        [Required(ErrorMessage = "Номер дома обязателен")]
        [StringLength(5, ErrorMessage = "Номер дома не должен превышать 5 символов")]
        public string DeliveryBuilding { get; set; } = null!;

        [Column("deliveryapartment")]
        [Range(1, 1000, ErrorMessage = "Номер квартиры должен быть от 1 до 1000")]
        public int? DeliveryApartment { get; set; }

        [Column("clientnotes")]
        [StringLength(300, ErrorMessage = "Заметки не должны превышать 300 символов")]
        public string? ClientNotes { get; set; }

        [Column("deliverydate")]
        [Required(ErrorMessage = "Дата доставки обязательна")]
        [FutureDate(ErrorMessage = "Дата доставки должна быть в будущем")]
        public DateTime DeliveryDate { get; set; }

        [Column("createdat")]
        [Required(ErrorMessage = "Дата создания обязательна")]
        public DateTime CreatedAt { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual OrderStatus Status { get; set; } = null!;
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
        public virtual PaymentStatus PaymentStatus { get; set; } = null!;
        public virtual DeliveryMethod DeliveryMethod { get; set; } = null!;

        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
