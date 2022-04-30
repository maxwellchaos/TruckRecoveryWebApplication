using System.ComponentModel.DataAnnotations;

namespace WebServiceTruckRecovery.Models
{
    /// <summary>
    /// Работа по ремонту
    /// </summary>
    public class Repair
    {
        [Required] //Обязательное поле
        public int Id { get; set; }
        /// <summary>
        /// название работы
        /// </summary>
        [Required]
        [Display(Name = "Работа")]
        public string Name { get; set; }

        /// <summary>
        /// стоимость работы
        /// </summary>
        [Required]
        [Display(Name = "Стоимость")]
        public int Price { get; set; }

        /// <summary>
        /// заказ в котором нужно выполнить эту работу
        /// </summary>
        public Order Order { get; set; }
    }
}
