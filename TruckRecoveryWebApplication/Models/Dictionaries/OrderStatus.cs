using System.ComponentModel.DataAnnotations;

namespace WebServiceTruckRecovery.Models
{
    /// <summary>
    /// статус заказа
    /// </summary>
    public class OrderStatus
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// название статуса
        /// </summary>
        [Required]
        [Display(Name = "Статус")]
        public string Status { get; set; }
        public List<Order>? Orders { get; set; }

    }
}
