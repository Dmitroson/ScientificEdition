using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Models.Review
{
    public class ArticleVersionReviewModel
    {
        public Guid ArticleVersionId { get; set; }

        public ArticleVersion? ArticleVersion { get; set; }

        [Required]
        [Display(Name = "Відгук")]
        public string? ReviewComment { get; set; }

        [Required]
        [Display(Name = "Рішення")]
        public ReviewResult Result { get; set; }
    }
}
