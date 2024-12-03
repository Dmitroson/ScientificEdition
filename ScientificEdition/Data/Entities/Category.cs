using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Data.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Article> Articles { get; set; } = [];

        public ICollection<User> Users { get; set; } = [];

        public ICollection<JournalEdition> Journals { get; set; } = [];
    }
}
