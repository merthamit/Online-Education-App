using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.Entity.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ImageUrl { get; set; }
        [JsonIgnore] public virtual List<Course> Courses { get; set; }
        [JsonIgnore] public virtual List<CourseRegister> CourseRegisters { get; set; }
        [JsonIgnore] public virtual List<Blog> Blogs { get; set; }
        [JsonIgnore] public virtual List<CourseReview> CourseReviews { get; set; }
        [JsonIgnore] public virtual List<TeacherApplication> TeacherApplications { get; set; }

        public virtual List<TeacherSocial> TeacherSocials { get; set; }

        // Kimlik bilgilerinin API yanıtlarında (kurs/blog listeleri vb.) dışarı sızmasını engeller
        [JsonIgnore] public override string? PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }
        [JsonIgnore] public override string? SecurityStamp { get => base.SecurityStamp; set => base.SecurityStamp = value; }
        [JsonIgnore] public override string? ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }
        [JsonIgnore] public override string? NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }
        [JsonIgnore] public override string? NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }
        [JsonIgnore] public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        [JsonIgnore] public override int AccessFailedCount { get => base.AccessFailedCount; set => base.AccessFailedCount = value; }
        [JsonIgnore] public override DateTimeOffset? LockoutEnd { get => base.LockoutEnd; set => base.LockoutEnd = value; }
    }
}
