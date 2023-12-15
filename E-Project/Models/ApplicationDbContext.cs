using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Project.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

             modelBuilder.Entity<Advertisement>()
            .HasOne(a => a.User)
            .WithMany(u => u.Advertisements)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Advertisement_User");
            
            modelBuilder.Entity<Driver>()
            .HasOne(a => a.User)
            .WithMany(u => u.Drivers)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Driver_User");

            modelBuilder.Entity<Company>()
           .HasOne(c => c.User)
           .WithMany(u => u.Companies)
           .HasForeignKey(c => c.UserId)
           .OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_Company_User"); ;

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey("LoginProvider", "ProviderKey");
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            ChangeTracker.LazyLoadingEnabled = true;
        }
       
    }
}
