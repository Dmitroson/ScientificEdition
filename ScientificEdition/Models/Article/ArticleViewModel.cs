using ScientificEdition.Data;
using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Models.Article
{
    public class ArticleViewModel
    {
        public Guid Id { get; set; }

        public required string Title { get; set; }

        public required string Category { get; set; }

        public DateTime UploadDate { get; set; }

        public ArticleStatus Status { get; set; }

        public string? Comment { get; set; }

        public List<ArticleVersionViewModel> Versions { get; set; } = [];
    }
}
