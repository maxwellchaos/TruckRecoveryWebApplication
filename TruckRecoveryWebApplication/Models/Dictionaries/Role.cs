using System.ComponentModel.DataAnnotations;

namespace TruckRecoveryWebApplication.Models
{
    /// <summary>
    /// справочник ролей - прав доступа
    /// </summary>
    public class Role
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Название роли
        /// </summary>
        [Required]
        [Display(Name = "Права")]

        public string Name { get; set; }

        public List<SystemUser>? Users { get; set; }
    }
}
