using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication1.Models.Domain;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book_Author>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.Book_Authors)
                .HasForeignKey(ba => ba.BookId);

            modelBuilder.Entity<Book_Author>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.Book_Authors)
                .HasForeignKey(ba => ba.AuthorId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherID);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book_Author> Books_Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
    }
}