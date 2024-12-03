using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Areas.Admin.Models.Journal
{
    public class JournalInputModel
    {
        public Guid Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
