using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleVersion> ArticleVersions { get; set; }
        public DbSet<Review> Reviews { get; set; }

        //public DbSet<PublicationJournal> PublicationJournals { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<User>()
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
