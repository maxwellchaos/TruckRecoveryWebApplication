using System.ComponentModel.DataAnnotations;

namespace WebServiceTruckRecovery.Models
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Client
    {
        [Required] //Обязательное поле
        public int Id { get; set; }

        /// <summary>
        /// Как обращаться к клиенту
        /// Обычно - ФИО
        /// </summary>
        [Required]
        [Display(Name = "Клиент")]//это нужно чтобы при генерации представления автоматически подставлялись эти названия
        public string Name { get; set; }

        /// <summary>
        /// телефон
        /// </summary>
        [Required]
        [Display(Name = "Телефон")] 
        public string Tel { get; set; }

        /// <summary>
        /// компания, если есть
        /// </summary>
        //на начальном этапе буду вводить просто строкой, в будущем, если понадобится, переделаю на отдельную таблицу
        [Display(Name = "Компания")] 
        public string? Company { get; set; }

        /// <summary>
        /// скидка
        /// </summary>
        [Display(Name = "Скидка")] 
        public int Discount { get; set; }

        public List<Order>? Orders { get; set; }

    }
}
