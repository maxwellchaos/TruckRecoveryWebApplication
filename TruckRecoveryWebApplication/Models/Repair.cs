using System.ComponentModel.DataAnnotations;

namespace TruckRecoveryWebApplication.Models
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
        public uint Price { get; set; }

        /// <summary>
        /// заказ в котором нужно выполнить эту работу
        /// </summary>
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
