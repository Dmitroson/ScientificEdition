using ScientificEdition.Mvc.Filters.Validation;
using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Areas.Admin.Models.Article
{
    public class ReviewersAssignmentModel
    {
        [Required]
        public Guid ArticleId { get; set; }

        public Data.Entities.Article? Article { get; set; }

        [Display(Name = "Доступні рецензенти")]
        [Required]
        [NonEmptyList(2)]
        public List<string> ReviewerIds { get; set; } = [];

        public bool MoveToReview { get; set; }
    }
}
