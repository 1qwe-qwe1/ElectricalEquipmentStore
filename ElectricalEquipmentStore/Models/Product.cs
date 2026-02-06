using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("productid")]
        public long ProductId { get; set; }

        [Column("sku")]
        [Required(ErrorMessage = "Артикул обязателен")]
        [StringLength(10, ErrorMessage = "Артикул не должен превышать 10 символов")]
        public string Sku { get; set; } = null!;

        [Column("name")]
        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
        public string Name { get; set; } = null!;

        [Column("description")]
        [StringLength(250, ErrorMessage = "Описание не должно превышать 250 символов")]
        public string? Description { get; set; }

        [Column("categoryid")]
        [ForeignKey("Category")]
        public long CategoryId { get; set; }

        [Column("manufacturerid")]
        [ForeignKey("Manufacturer")]
        public long ManufacturerId { get; set; }

        [Column("image")]
        [Required(ErrorMessage = "Изображение обязательно")]
        [StringLength(50, ErrorMessage = "Путь к изображению не должен превышать 50 символов")]
        public string Image { get; set; } = null!;

        [Column("price")]
        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 1000000, ErrorMessage = "Цена должна быть от 0.01 до 1,000,000")]
        public decimal Price { get; set; }

        [Column("currentdiscount")]
        [Range(0, 100, ErrorMessage = "Скидка должна быть от 0 до 100%")]
        public long? CurrentDiscount { get; set; }

        [Column("stockquantity")]
        [Required(ErrorMessage = "Количество на складе обязательно")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int StockQuantity { get; set; }

        [Column("statusid")]
        [ForeignKey("Status")]
        public long StatusId { get; set; }

        [Column("createdat")]
        [Required(ErrorMessage = "Дата создания обязательна")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedat")]
        public DateTime? UpdatedAt { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Manufacturer Manufacturer { get; set; } = null!;
        public virtual ProductStatus Status { get; set; } = null!;

        public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<SupplyProduct> SupplyProducts { get; set; } = new List<SupplyProduct>();
    }
}
