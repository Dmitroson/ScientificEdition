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
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public string? Comment { get; set; }

        [Required]
        public DateTime UploadDate { get; set; }

        [Required]
        public ArticleStatus Status { get; set; }

        [Required]
        public required string AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User? Author { get; set; }

        public ICollection<User> Reviewers { get; set; } = [];

        public ICollection<ArticleVersion> Versions { get; set; } = [];

        public Guid? JournalEditionId { get; set; }

        [ForeignKey(nameof(JournalEditionId))]
        public JournalEdition? JournalEdition { get; set; }
    }
}
