using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JWTAuthApplication.Models
{
    public partial class USERDESKContext : DbContext
    {
        public USERDESKContext()
        {
        }

        public USERDESKContext(DbContextOptions<USERDESKContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Usertable> Usertable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=USERDESK;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usertable>(entity =>
            {
                entity.ToTable("USERTABLE");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasColumnName("EmailID")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.IsAdmin)
                    .HasColumnName("isAdmin")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
        }
    }
}
