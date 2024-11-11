using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Models
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int ReviewId { get; set; }
        public string ReviewContent { get; set; }
        public int ArticleId { get; set; }
        public string ReviewerName { get; set; }
    }
}
