using Microsoft.AspNetCore.Identity;

namespace ScientificEdition.Data.Entities
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Category> Categories { get; set; } = [];

        public ICollection<Article> Articles { get; set; } = [];

        public ICollection<Article> AssignedArticles { get; set; } = [];
    }
}
