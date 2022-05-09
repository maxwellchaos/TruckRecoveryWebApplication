using System.ComponentModel.DataAnnotations;
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
        /// информация о пользователе, совершившем событие
        /// </summary>
        [Required]
        [Display(Name = "Кто совершил событие")]
        public int UserId { get; set; }

        /// <summary>
        /// информация о пользователе, совершившем событие
        /// </summary>
        [Display(Name = "Кто совершил событие")]
        public SystemUser? User { get; set; }


        /// <summary>
        /// Дата и время когда случилось событие
        /// </summary>
        [Display(Name = "Дата и время события")]
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// ссылка на объект заказа, в котором произошло событие
        /// </summary>
        public int OrderId { get; set; }
        public Order Order { get; set; }


        /// <summary>
        /// Добавляет событие о заказе. Не сохраняет в БД, сохранять нужно отдельно
        /// </summary>
        /// <param name="OrdrId"> ID заказа</param>
        /// <param name="Event"> Текст события, добавляемый в журнал</param>
        /// <param name="context">Контекст БД</param>
        /// <param name="UserId">Идентификатор юзера, инициирующего событие</param>
        public static void AddLog(int OrdrId, string Event, Context context, int UserId)
        {
            Log log = new Log();
            log.OrderId = OrdrId;
            log.Event = Event;
            log.UserId = UserId;
            log.EventDateTime = DateTime.Now;
            context.Add(log);
        }
    }
}
