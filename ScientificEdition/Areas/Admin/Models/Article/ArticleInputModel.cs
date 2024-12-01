using ScientificEdition.Data;
using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Areas.Admin.Models.Article
{
    public class ArticleInputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле обов'язкове.")]
        [StringLength(200, ErrorMessage = "Заголовок не може перевищувати {1} символів.")]
        [Display(Name = "Заголовок")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Поле обов'язкове.")]
        [Display(Name = "Категорія")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Поле обов'язкове.")]
        [Display(Name = "Статус")]
        public ArticleStatus Status { get; set; }

        [Display(Name = "Коментар")]
        public string? Comment { get; set; }
    }
}
