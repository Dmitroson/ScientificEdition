using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScientificEdition.Data.Entities
{
    public class ArticleVersion
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Range(1, short.MaxValue)]
        public required int VersionNumber { get; set; }

        [Required]
        public required string FilePath { get; set; }

        public string? Comment { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public Guid ArticleId { get; set; }

        [ForeignKey(nameof(ArticleId))]
        public Article? Article { get; set; }

        public ICollection<Review> Reviews { get; set; } = [];
    }
}
