using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.Domain.Authorization;

using Microsoft.EntityFrameworkCore;

namespace HealthInsuranceSystem.Core.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AuthClaim> AuthClaims { get; set; }
        public virtual DbSet<RoleAuthClaim> RoleAuthClaims { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<ClaimsAudit> ClaimsAudit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.LastName)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.NationalID)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.UserPolicyNumber)
                   .IsRequired()
                   .IsUnicode(false);

                entity.Property(e => e.DateOfBirth)
                   .IsRequired();


            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(200)
                   .IsUnicode(false);

                entity.Property(e => e.RoleType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                   .IsRequired();

                entity.Property(e => e.ModifiedDate)
                   .IsRequired();
            });

            modelBuilder.Entity<AuthClaim>(entity =>
            {
                entity.HasKey(e => e.ClaimId);

                entity.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);


                entity.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(200)
                   .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                   .IsRequired();
            });

            modelBuilder.Entity<RoleAuthClaim>(entity =>
            {
                entity.HasKey(e => e.RoleClaimId);

                entity.Property(e => e.Active)
                     .IsRequired();
            });
        }
    }
}
