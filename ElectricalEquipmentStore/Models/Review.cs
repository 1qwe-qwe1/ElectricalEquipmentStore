using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalEquipmentStore.Models
{
    [Table("reviews")]
    public class Review
    {
        [Key]
        [Column("reviewid")]
        public long ReviewId { get; set; }

        [Column("productid")]
        [ForeignKey("Product")]
        public long ProductId { get; set; }

        [Column("clientid")]
        [ForeignKey("Client")]
        public long ClientId { get; set; }

        [Column("orderid")]
        [ForeignKey("Order")]
        public long OrderId { get; set; }

        [Column("title")]
        [StringLength(50, ErrorMessage = "Заголовок не должен превышать 50 символов")]
        public string? Title { get; set; }

        [Column("text")]
        [StringLength(500, ErrorMessage = "Текст отзыва не должен превышать 500 символов")]
        public string? Text { get; set; }

        [Column("image")]
        [StringLength(50, ErrorMessage = "Путь к изображению не должен превышать 50 символов")]
        public string? Image { get; set; }

        [Column("grade")]
        [Required(ErrorMessage = "Оценка обязательна")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public short Grade { get; set; }

        [Column("createdat")]
        [Required(ErrorMessage = "Дата создания обязательна")]
        public DateTime CreatedAt { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Client Client { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
