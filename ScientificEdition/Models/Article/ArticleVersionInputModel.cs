using ScientificEdition.Mvc.Filters.Validation;
using System.ComponentModel.DataAnnotations;
using static ScientificEdition.Business.Constants.Validation;

namespace ScientificEdition.Models.Article
{
    public class ArticleVersionInputModel
    {
        public required Guid ArticleId { get; set; }

        [Required(ErrorMessage = "Файл обов'язковий.")]
        [AllowedExtensions([".docx", ".doc", ".pdf"], [FileContentTypes.Docx, FileContentTypes.Doc, FileContentTypes.Pdf])]
        [Display(Name = "Файл")]
        public IFormFile? File { get; set; }

        [Display(Name = "Коментар")]
        public string? Comment { get; set; }
    }
}
