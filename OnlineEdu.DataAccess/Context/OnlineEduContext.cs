using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.DataAccess.Context
{
    public class OnlineEduContext: IdentityDbContext<AppUser, AppRole, int>
    {
        public OnlineEduContext(DbContextOptions options): base(options) 
        {

        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<CourseRegister> CourseRegisters { get; set; }
        public DbSet<TeacherSocial> TeacherSocials { get; set; }
        public DbSet<CourseVideo> CourseVideos { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<TeacherApplication> TeacherApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Bir öğrenci bir kursa yalnızca bir kez puan verebilir
            builder.Entity<CourseReview>()
                .HasIndex(x => new { x.CourseId, x.AppUserId })
                .IsUnique();

            builder.Entity<CourseReview>()
                .Property(x => x.Comment).HasMaxLength(1000);

            builder.Entity<TeacherApplication>(e =>
            {
                e.Property(x => x.Expertise).HasMaxLength(200);
                e.Property(x => x.Motivation).HasMaxLength(2000);
                e.Property(x => x.PortfolioUrl).HasMaxLength(300);
                e.Property(x => x.AdminNote).HasMaxLength(500);
                e.HasIndex(x => new { x.AppUserId, x.Status });
            });
        }
    }
}
