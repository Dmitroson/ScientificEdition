using ScientificEdition.Mvc.Filters.Validation;
using System.ComponentModel.DataAnnotations;
using static ScientificEdition.Business.Constants.Validation;

namespace ScientificEdition.Models.Article
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
        public required string Category { get; set; }

        [Required(ErrorMessage = "Файл обов'язковий.")]
        [AllowedExtensions([".docx", ".doc", ".pdf"], [FileContentTypes.Docx, FileContentTypes.Doc, FileContentTypes.Pdf])]
        [Display(Name = "Файл")]
        public required IFormFile File { get; set; }

        [Display(Name = "Коментар")]
        public string? Comment { get; set; }
    }
}
