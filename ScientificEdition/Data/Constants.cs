namespace ScientificEdition.Data
{
    public class UserRoles
    {
        public const string Author = "Author";
        public const string Admin = "Admin";
        public const string Reviewer = "Reviewer";
    }

    public enum ArticleStatus
    {
        New,
        Review,
        Rework,
        Approved,
        Published,
        Cancelled
    }
}
