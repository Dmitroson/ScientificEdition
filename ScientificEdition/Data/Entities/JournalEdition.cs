using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScientificEdition.Data.Entities
{
    public class JournalEdition
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public bool Published { get; set; }

        public DateTime? PublishDate { get; set; }

        public ICollection<Article> Articles { get; set; } = [];
    }
}
