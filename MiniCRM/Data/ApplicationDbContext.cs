using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Models;

namespace MiniCRM.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<EmailMessage> EmailMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Contact>(e =>
            {
                e.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                e.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                e.Property(p => p.Email).HasMaxLength(256);
                e.Property(p => p.Company).HasMaxLength(100);

                e.HasMany(c => c.EmailMessages)
                 .WithOne(m => m.Contact!)
                 .HasForeignKey(m => m.ContactId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<EmailMessage>(e =>
            {
                e.Property(p => p.To).IsRequired().HasMaxLength(256);
                e.Property(p => p.Subject).IsRequired().HasMaxLength(200);
                e.Property(p => p.Body).IsRequired();
            });
        }
    }
}