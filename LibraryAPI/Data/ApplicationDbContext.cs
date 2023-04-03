using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>()
                .HasMany(bc => bc.BookCategories)
                .WithOne(b => b.Book)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Category>()
                .HasMany(bc => bc.BookCategories)
                .WithOne(c => c.Category)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
