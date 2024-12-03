using ScientificEdition.Data.Entities;

namespace ScientificEdition.Models.Journal
{
    public class JournalIndexViewModel
    {
        public required Category Category { get; set; }

        public List<JournalEdition> Journals { get; set; } = [];
    }
}
