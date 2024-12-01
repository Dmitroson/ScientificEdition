using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScientificEdition.Data.Entities
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ArticleVersionId { get; set; }

        [ForeignKey(nameof(ArticleVersionId))]
        public ArticleVersion? ArticleVersion { get; set; }

        [Required]
        public required string Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [Required]
        public required string ReviewerId { get; set; }

        [ForeignKey(nameof(ReviewerId))]
        public User? Reviewer { get; set; }
    }
}
