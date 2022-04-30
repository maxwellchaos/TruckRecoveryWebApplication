using System.ComponentModel.DataAnnotations;
using TruckRecoveryWebApplication.Models;

namespace WebServiceTruckRecovery.Models
{
    /// <summary>
    /// Заказ/заявка
    /// </summary>
    public class Order
    {
        [Required] //Обязательное поле
        public int Id { get; set; }

        /// <summary>
        /// дата создания заказа
        /// </summary>

        [Required]
        [Display(Name = "Дата создания заказа")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// клиент
        /// </summary>

        [Required]
        [Display(Name = "Клиент")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        /// <summary>
        /// номер заказа
        /// </summary>
        // брать из бумажных документов
        //может быть нулевым
        
        [Display(Name = "Номер заказа")]

        public string? OrderNumber { get; set; }

        /// <summary>
        /// статус заказа
        /// </summary>
   
        [Required]
        [Display(Name = "Статус заказа")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// список оборудования, переданного на диагностику или подлежащего ремонту
        /// </summary>
    
        [Display(Name = "Оборудование")]
        public string? TruckList { get; set; }


        /// <summary>
        /// дата проведения диагностики
        /// </summary>
        [Display(Name = "Дата диагностики")]
        public DateTime? DiagnosticsDate { get; set; }

        /// <summary>
        /// отчет по диагностике
        /// </summary>
        //сейчас в текстовом виде
        //возможно проще будет сделать ссылку на фото отчета по диагностике
        [Display(Name = "Отчет по диагностике")]
        public string? DiagnosticReport { get; set; }

        /// <summary>
        /// Список необходимых запчастей
        /// </summary>
        [Display(Name = "Требуемые запчасти")]
        public List<SparePartsList> SparePartsList { get; set; }

        /// <summary>
        /// список работ по ремонту
        /// </summary>
        [Display(Name = "Требуемые работы")] 
        public List<Repair> Repairs { get; set; }

        /// <summary>
        /// Цена на работу и запчасти в сумме
        /// </summary>
        [Display(Name = "Цена")]
        public int Price { get; set; }

        /// <summary>
        /// Цена со скидкой. Рассчитывается автоматически из Скидки клиента и цены
        /// </summary>
        [Display(Name = "Цена со скидкой")]
        public int DiscountedPrice { get; set; }

        /// <summary>
        /// дата доставки последних запчастей
        /// </summary>
        [Display(Name = "Дата доставки всех запчастей")]
        public DateTime DeliveryPartsDate { get; set; }

        /// <summary>
        /// дата закрытия заказа
        /// </summary>
        [Display(Name = "Дата закрытия заказа")]
        public DateTime CloseDate { get; set; }

        /// <summary>
        /// заявка закрыта
        /// </summary>
        [Display(Name = "Заказ закрыт")]
        public bool IsClosed { get; set; }

        public List<Log> Logs { get; set; }

    }
}
