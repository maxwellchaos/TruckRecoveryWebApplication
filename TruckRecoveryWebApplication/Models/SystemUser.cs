using System.ComponentModel.DataAnnotations;

namespace WebServiceTruckRecovery.Models
{
    /// <summary>
    /// пользователь системы
    /// </summary>
    public class SystemUser
    {
        [Required] //Обязательное поле
        public int Id { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }
        
        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }
        
        [Required]
        [Display(Name = "Права")]
        public Role Role { get; set; }

    }
}
