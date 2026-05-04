using Microsoft.EntityFrameworkCore;
using JWTApi.Domain.Entities;
using System.Reflection.Metadata;

namespace JWTApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();





        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<IpLock> IpLocks { get; set; }

        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }

        public DbSet<Company> Companies { get; set; }



        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Attachment>()
    .HasKey(d => d.Id);


       



            modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
            modelBuilder.Entity<LoginAttempt>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
                b.Property(x => x.Username).HasMaxLength(200);
                b.Property(x => x.Reason).HasMaxLength(200);
                b.HasIndex(x => new { x.UserId, x.AttemptTime });
                b.HasIndex(x => x.AttemptTime);
            });

            modelBuilder.Entity<IpLock>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
                b.HasIndex(x => x.IPAddress).IsUnique();
            });
            modelBuilder.Entity<Role>(b =>
            {

                b.Property(x => x.Name).HasMaxLength(250).IsRequired();

            });
            // ---------------- User ----------------
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Name).HasMaxLength(100).IsRequired();
                b.Property(u => u.Email).HasMaxLength(200);
                b.Property(u => u.FullName).HasMaxLength(200);
                b.Property(u => u.MobileNumber).HasMaxLength(200).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(r => r.IsActive).HasDefaultValue(true);
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");
          


            

            });

            // ---------------- Project ----------------
            modelBuilder.Entity<Project>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.HasOne(rm => rm.Company)
            .WithMany(m => m.Projects)
            .HasForeignKey(rm => rm.CompanyId)
              .OnDelete(DeleteBehavior.Restrict); // اضافه کنید

            });
            // ---------------- Company ----------------
            modelBuilder.Entity<Company>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
                b.Property(p => p.IsHolding).HasDefaultValueSql("0");
                b.Property(p => p.IsDeleted).HasDefaultValueSql("0");
                b.Property(p => p.CreateAt).HasDefaultValueSql("GETDATE()");


            });
            //---------------Bank
            modelBuilder.Entity<Bank>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Name).HasMaxLength(200).IsRequired();
          
                b.Property(p => p.CreateAt).HasDefaultValueSql("GETDATE()");


            });


            //--------------

            modelBuilder.Entity<ProjectUser>(b =>
            {
                b.HasKey(p => p.Id);

            });
            // ---------------- Package ----------------



            // ---------------- ExtraProject ----------------

            // ---------------- Todo ----------------

            //-----------------TagProject ---------------


            //---------------RoleMenu --------------

            modelBuilder.Entity<RoleMenu>(b =>
            {
                b.HasKey(rm => new { rm.RoleId, rm.MenuId });

                b.HasOne(rm => rm.Role)
                 .WithMany(r => r.RoleMenus)
                 .HasForeignKey(rm => rm.RoleId);

                b.HasOne(rm => rm.Menu)
                 .WithMany(m => m.RoleMenus)
                 .HasForeignKey(rm => rm.MenuId);
            });

            //---------------FinancialCompany --------------

            modelBuilder.Entity<FinancialCompany>(b =>
            {
                b.HasKey(rm => new { rm.FinancialId, rm.CompanyId });

                b.HasOne(rm => rm.Company)
                 .WithMany(r => r.FinancialCompanies)
                 .HasForeignKey(rm => rm.CompanyId);

                b.HasOne(rm => rm.Financial)
                 .WithMany(m => m.FinancialCompanies)
                 .HasForeignKey(rm => rm.FinancialId);
            });
            //---------------FinancialCompany --------------

            modelBuilder.Entity<BankCompany>(b =>
            {
                b.HasKey(rm => new { rm.BankId, rm.CompanyId });

                b.HasOne(rm => rm.Company)
                 .WithMany(r => r.BankCompanies)
                 .HasForeignKey(rm => rm.CompanyId);

                b.HasOne(rm => rm.Bank)
                 .WithMany(m => m.BankCompanies)
                 .HasForeignKey(rm => rm.BankId);
            });

            // ---------------- Menu ----------------

        }
    }
}
