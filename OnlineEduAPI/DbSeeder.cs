using OnlineEdu.DataAccess.Context;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI
{
    public static class DbSeeder
    {
        public static void Seed(OnlineEduContext context)
        {
            if (!context.Abouts.Any())
            {
                context.Abouts.Add(new About { Description = "Online Eğitim Platformumuza Hoş Geldiniz!", ImageUrl1 = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?q=80&w=1471&auto=format&fit=crop", ImageUrl2 = "https://images.unsplash.com/photo-1524178232363-1fb2b075b655?q=80&w=1470&auto=format&fit=crop", Item1 = "Uzman Eğitmenler", Item2 = "Ömür Boyu Erişim", Item3 = "Sertifikalı Kurslar", Item4 = "7/24 Destek" });
            }

            if (!context.CourseCategories.Any())
            {
                context.CourseCategories.AddRange(
                    new CourseCategory { CategoryName = "Yazılım Geliştirme", Icon = "fa fa-code", Description = "Web, mobil ve oyun geliştirme", IsShown = true },
                    new CourseCategory { CategoryName = "Tasarım", Icon = "fa fa-paint-brush", Description = "UI/UX ve Grafik Tasarım", IsShown = true },
                    new CourseCategory { CategoryName = "İş Dünyası", Icon = "fa fa-briefcase", Description = "Girişimcilik ve Finans", IsShown = true }
                );
                context.SaveChanges();
            }

            if (!context.Courses.Any())
            {
                int categoryId = context.CourseCategories.First().CourseCategoryId;
                context.Courses.AddRange(
                    new Course { CourseName = "Sıfırdan C# ve .NET Core", ImageUrl = "https://images.unsplash.com/photo-1550439062-609e1531270e?q=80&w=1470&auto=format&fit=crop", Price = 299.99m, IsShown = true, CourseCategoryId = categoryId },
                    new Course { CourseName = "Modern Web Geliştirme (React & Node.js)", ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?q=80&w=1470&auto=format&fit=crop", Price = 349.99m, IsShown = true, CourseCategoryId = categoryId }
                );
            }

            if (!context.Banners.Any())
            {
                context.Banners.Add(new Banner { Title = "Geleceğinize Yatırım Yapın", ImageUrl = "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?q=80&w=1470&auto=format&fit=crop" });
            }

            if (!context.BlogCategories.Any())
            {
                context.BlogCategories.AddRange(
                    new BlogCategory { Name = "Yazılım Teknolojileri" },
                    new BlogCategory { Name = "Kariyer Tavsiyeleri" }
                );
                context.SaveChanges();
            }

            if (!context.Blogs.Any())
            {
                int blogCategoryId = context.BlogCategories.First().BlogCategoryId;
                context.Blogs.AddRange(
                    new Blog { Title = "2024 Yılında Öğrenilmesi Gereken 5 Programlama Dili", Content = "Teknoloji hızla değişiyor. Bu yıl en çok tercih edilen diller...", BlogCategoryId = blogCategoryId, BlogDate = System.DateTime.Now, ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?q=80&w=1470&auto=format&fit=crop" }
                );
            }
            
            if(!context.Testimonials.Any())
            {
                context.Testimonials.AddRange(
                    new Testimonial { Name = "Ahmet Yılmaz", Title = "Yazılım Geliştirici", Comment = "Harika bir platform!", ImageUrl = "https://randomuser.me/api/portraits/men/32.jpg", Star = 5 }
                );
            }

            if(!context.Contacts.Any())
            {
                context.Contacts.Add(new Contact { Phone = "+90 555 123 4567", Email = "info@onlineedu.com", Address = "Bilişim Vadisi, Kocaeli", MapUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3013.3134659850233!2d29.458925575510675!3d40.79913163223018!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x14cad198db4ab695%3A0xc3cf93fc5eb04d55!2zQmlsacWfaW0gVmFkaXNp!5e0!3m2!1str!2str!4v1714856000000!5m2!1str!2str" });
            }

            context.SaveChanges();
        }
    }
}
