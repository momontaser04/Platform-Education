using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PlatformEduPro.Contracts.Configuration;
using PlatformEduPro.Contracts.IdentityConfiguration;
using PlatformEduPro.Models;

namespace PlatformEduPro.Models
{
    public class EduPlatformDbContext: IdentityDbContext<AppUser,AppRole,string>
    {
        public EduPlatformDbContext(DbContextOptions<EduPlatformDbContext> options) : base(options)
        {
            
        }

        public DbSet<TbImages> TbImages { get; set; }
        public DbSet<TbStudentCourses> TbStudentCourses { get; set; }
        public DbSet<TbCourses> TbCourses { get; set; }
        public DbSet<TbExam> tbExams { get; set; }
        public DbSet<TbChoices> TbChoices { get; set; }
        public DbSet<TbQuestions> TbQuestions { get; set; }

        public DbSet<TbVideoes> TbVideoes { get; set; }
        public DbSet<TbSections> TbSections { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>()
                         .OwnsMany(x => x.RefreshTokens, rt =>
                         {
                             rt.ToTable("RefreshToken");

                             rt.WithOwner().HasForeignKey("UserId");

                             rt.HasKey("UserId", "Id"); 

                             rt.Property<int>("Id")
                                 .ValueGeneratedOnAdd()
                                 .HasAnnotation("SqlServer:Identity", "1, 1");
                         });


            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
