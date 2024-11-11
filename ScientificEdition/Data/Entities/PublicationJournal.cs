namespace ScientificEdition.Data.Entities
{
    public class PublicationJournal
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        public string Author { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }
    }
}
