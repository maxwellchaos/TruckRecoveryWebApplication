﻿using System.ComponentModel.DataAnnotations;
using WebServiceTruckRecovery.Models;

namespace TruckRecoveryWebApplication.Models
{
    /// <summary>
    /// класс журналирования всех действий пользователей системы
    /// </summary>
    public class Log
    {
        /// <summary>
        /// идентификатор
        /// </summary>
        [Required] //Обязательное поле
        public int Id { get; set; }

        /// <summary>
        /// информация о логируемом событии
        /// </summary>
        [Required]
        [Display(Name = "Событие")]//это нужно чтобы при генерации представления автоматически подставлялись эти названия
        public string Event { get; set; }

        /// <summary>
        /// Дата и время когда случилось событие
        /// </summary>
        [Display(Name = "Дата и время события")]
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// ссылка на объект заказа, в котором произошло событие
        /// </summary>
        public Order Order { get; set; }
    }
}