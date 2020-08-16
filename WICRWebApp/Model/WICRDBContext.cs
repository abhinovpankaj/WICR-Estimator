using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WICRWebApp.Model
{
    public partial class WICRDBContext : DbContext
    {
        public WICRDBContext()
        {
        }

        public WICRDBContext(DbContextOptions<WICRDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Materials> Materials { get; set; }
        public virtual DbSet<Project> Project { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:wicrappdb.database.windows.net,1433;Initial Catalog=wicrproject;Persist Security Info=False;User ID=priyanka;Password=Chopan@91;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Materials>(entity =>
            {
                entity.HasKey(e => e.MaterialId)
                    .HasName("PK__Material__C506131755F6270A");

                entity.Property(e => e.MaterialId).ValueGeneratedNever();

                entity.Property(e => e.MaterialName).IsUnicode(false);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Createdby).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);
            });
        }
    }
}
