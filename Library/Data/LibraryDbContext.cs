using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<BookBaseModel> Books { get; set; }
        public DbSet<GenreBaseModel> Genre { get; set; }
        public DbSet<UserBaseModel> User { get; set; }
        public DbSet<Loan> Prestiti { get; set; }
        public DbSet<LoanBooks> LibriPrestati { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookBaseModel>()
                .HasOne(b => b.Genre) //nome della proprità in Book che fa riferimento ai generi
                .WithMany(b => b.BookCollection);//nome della collection in generi che fa riferimento ai books

            modelBuilder.Entity<LoanBooks>()
                            .HasIndex(lp => new { lp.IdBook, lp.IsReturned })
                            .HasFilter("[IsReturned] = 0")
                            .IsUnique();

        }

    }
}
