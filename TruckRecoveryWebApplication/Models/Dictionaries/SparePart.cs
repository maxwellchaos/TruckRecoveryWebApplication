using System.ComponentModel.DataAnnotations;

namespace TruckRecoveryWebApplication.Models
{
    /// <summary>
    /// запчасть
    /// </summary>
    public class SparePart
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// название запчасти
        /// </summary>
        [Required]
        [Display(Name = "Запчасть")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Цена запчасти")]
        public int Price { get; set; }        
        public List<SparePartsList>? PartLists { get; set; }
    }
}
