using Microsoft.EntityFrameworkCore;
namespace booksReviews.Db
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Entities> Books { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=PAASHKOVAAA;Database=booksData;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}