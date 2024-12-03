using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Areas.Admin.Models.Article
{
    public class AddArticleToJournalInputModel
    {
        [Required]
        public Guid ArticleId { get; set; }

        [Required]
        public Guid JournalId { get; set; }

        public required Data.Entities.Article Article { get; set; }
    }
}
