using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryMs.Models;

namespace LibraryMs.Data
{
    public class LibraryMsContext : DbContext
    {
        public LibraryMsContext  (DbContextOptions<LibraryMsContext> options)
            : base(options)
        {
        }

        public DbSet<LibraryMs.Models.Form> Form { get; set; }
        public DbSet<LibraryMs.Models.Book> Book { get; set; }
        public DbSet<LibraryMs.Models.Student> Student { get; set; }
        public DbSet<LibraryMs.Models.Borrowing> Borrowing { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<Borrowing>()
                        .HasOne<Student>(s => s.CurrentStudent)
                        .WithMany(g => g.Borrowings)
                        .HasForeignKey(s => s.StudentID)
                        .OnDelete(DeleteBehavior.Cascade); // <= This entity has cascading behaviour on deletion

            modelBuilder.Entity<Borrowing>()
                        .HasOne<Book>(e => e.CurrentBook)
                        .WithMany(g => g.Borrowings)
                        .HasForeignKey(e => e.BookID)
                        .OnDelete(DeleteBehavior.Restrict); // <= This entity has restricted behaviour on deletion
        }

    }
}
