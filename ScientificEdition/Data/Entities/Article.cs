using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScientificEdition.Data.Entities
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Category { get; set; }

        public string? Comment { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public ArticleStatus Status { get; set; }

        [Required]
        public required string AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User? Author { get; set; }

        public ICollection<ArticleVersion> Versions { get; set; } = [];
    }
}
