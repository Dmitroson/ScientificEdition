using ScientificEdition.Data;

namespace ScientificEdition.Models.Article
{
    public class ArticleVersionViewModel
    {
        public int VersionId { get; set; }

        public int VersionNumber { get; set; }

        public ArticleStatus ArticleStatus { get; set; }

        public DateTime UploadDate { get; set; }

        public string? Comment { get; set; }
    }
}
