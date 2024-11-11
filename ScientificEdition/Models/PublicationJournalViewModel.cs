using ScientificEdition.Models.Article;
using System;
using System.Collections.Generic;

namespace ScientificEdition.Models
{
    public class PublicationJournalViewModel
    {
        // ID статті
        public int ArticleId { get; set; }

        // Назва статті
        public string Title { get; set; }

        // Ім'я автора статті
        public string AuthorName { get; set; }

        // Дата публікації
        public DateTime? PublicationDate { get; set; }

        // Категорія статті
        public string Category { get; set; }

        // Статус рецензії
        public string ReviewStatus { get; set; }

        // Список версій статті
        public List<ArticleVersionViewModel> Versions { get; set; } = new List<ArticleVersionViewModel>();

        // Опис статті або короткий зміст
        public string Description { get; set; }
    }
}
