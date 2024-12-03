using ScientificEdition.Data.Entities;

namespace ScientificEdition.Areas.Admin.Models.Journal
{
    public class JournalPublishInputModel
    {
        public Guid JournalId { get; set; }

        public bool PublishNow { get; set; }

        public DateTime? PublishDate { get; set; }

        public JournalEdition? Journal { get; set; }
    }
}
