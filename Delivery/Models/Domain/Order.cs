using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Domain
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Имя может содержать только буквы и пробелы.")]
        public string Name { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Вес должен быть до 100 кг")]
        public double Weight { get; set; }
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Район может содержать только буквы и пробелы.")] // Изменено на латинские буквы
        public string District { get; set; }
        [Required]
        public DateTime DeliveryDateTime { get; set; }
    }

}
