using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [Range(0, 100)]
        public double Weight { get; set; }
        [Required]
        [MaxLength(100)]
        public string District { get; set; }
        [Required]
        public DateTime DeliveryDateTime { get; set; }

    }
}
