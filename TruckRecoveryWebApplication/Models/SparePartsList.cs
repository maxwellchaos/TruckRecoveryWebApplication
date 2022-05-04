using System.ComponentModel.DataAnnotations;

namespace WebServiceTruckRecovery.Models
{
    /// <summary>
    /// список запчастей для ремонта конкретного техническоко средства
    /// </summary>
    public class SparePartsList
    {
        [Required] //Обязательное поле
        public int Id { get; set; }

        /// <summary>
        /// запчасть или узел для ремонта
        /// </summary>
        [Required]
        [Display(Name = "Запчасть")]
        public int SparePartId { get; set; }

        [Display(Name = "Запчасть")]
        public SparePart? SparePart { get; set; }
        

        /// <summary>
        /// количество запчастей
        /// </summary>
        [Required]
        [Display(Name = "Требуемое количество")]
        public int Count { get; set; }

        /// <summary>
        /// Запчасть уже на складе
        /// признак да/нет
        /// </summary>
        
        [Display(Name = "Наличие на складе")]
        public bool OnStock { get; set; }

        /// <summary>
        /// дата доставки запчасти на склад
        /// </summary>
       
        [Display(Name = "Дата доставки")]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "Код заказа")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

    }
}
