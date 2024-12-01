using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Areas.Admin.Models.Category
{
    public class CategoryInputModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Назва")]
        public required string Name { get; set; }
    }
}
