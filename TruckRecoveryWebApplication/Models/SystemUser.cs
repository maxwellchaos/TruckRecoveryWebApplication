using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckRecoveryWebApplication.Models
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


        [Display(Name = "Имя пользователя")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }
        
        [Required]
        [Display(Name = "Права")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        /// <summary>
        /// Это поле только для переадресации на нужную страницу сразу после ввода логина
        /// </summary>
        /// отсутствует в БД
        [NotMapped]
        public string? ReturnUrl { get; set; }

    }
}
