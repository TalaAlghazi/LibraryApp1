using Microsoft.EntityFrameworkCore;

namespace LibraryApp1.DataAccess
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<User> Users { get; set; }

        public LibraryDbContext()
        {
        }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=LibraryApp1;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
           .HasOne(r => r.Book)
           .WithMany()
           .HasForeignKey(r => r.BookId)
           .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", IsAvailable = true },
                new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", IsAvailable = true },
                new Book { Id = 3, Title = "1984", Author = "George Orwell", IsAvailable = true },
                new Book { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", IsAvailable = true },
                new Book { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", IsAvailable = true },
                new Book { Id = 6, Title = "Brave New World", Author = "Aldous Huxley", IsAvailable = true },
                new Book { Id = 7, Title = "Jane Eyre", Author = "Charlotte Bronte", IsAvailable = true },
                new Book { Id = 8, Title = "Wuthering Heights", Author = "Emily Bronte", IsAvailable = true }
            );
            // A book can have only one open reservation (pending, out, or waiting for return).
            // The database enforces it, so two people can never reserve the same copy at once.
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.BookId)
                .IsUnique()
                .HasFilter("[ReturnedAt] IS NULL");
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}