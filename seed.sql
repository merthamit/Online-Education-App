USE OnlineEduDb;
INSERT INTO BlogCategories (Name) VALUES ('Yapay Zeka'), ('Siber Güvenlik'), ('Web Geliştirme');
DECLARE @Cat1 INT = (SELECT TOP 1 BlogCategoryId FROM BlogCategories WHERE Name = 'Yapay Zeka');
DECLARE @Cat2 INT = (SELECT TOP 1 BlogCategoryId FROM BlogCategories WHERE Name = 'Siber Güvenlik');
INSERT INTO Blogs (Title, Content, ImageUrl, BlogDate, BlogCategoryId) VALUES 
('Yapay Zeka Geleceği Ele mi Geçiriyor?', 'Yapay zeka modellerinin son yıllardaki inanılmaz gelişimi...', 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=500&auto=format&fit=crop', GETDATE(), @Cat1),
('2026 Yılında Siber Güvenlik', 'Her yıl artan siber saldırılar şirketleri zorluyor...', 'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=500&auto=format&fit=crop', GETDATE(), @Cat2),
('Web Geliştirmede Yeni Trendler', 'Frontend dünyası hızla değişiyor. React ve Angular rekabeti...', 'https://images.unsplash.com/photo-1498050108023-c5249f4df085?w=500&auto=format&fit=crop', GETDATE(), (SELECT TOP 1 BlogCategoryId FROM BlogCategories WHERE Name = 'Web Geliştirme'));

INSERT INTO CourseCategories (CategoryName, Icon, Description, IsShown) VALUES 
('Veri Bilimi', 'fas fa-database', 'Veri madenciliği ve ML kursları', 1),
('Mobil Geliştirme', 'fas fa-mobile-alt', 'Android ve iOS geliştirme', 1);
DECLARE @CCat1 INT = (SELECT TOP 1 CourseCategoryId FROM CourseCategories WHERE CategoryName = 'Veri Bilimi');
DECLARE @CCat2 INT = (SELECT TOP 1 CourseCategoryId FROM CourseCategories WHERE CategoryName = 'Mobil Geliştirme');
INSERT INTO Courses (CourseName, ImageUrl, CourseCategoryId, Price, IsShown) VALUES 
('Python ile Veri Bilimine Giriş', 'https://images.unsplash.com/photo-1518770660439-4636190af475?w=500&auto=format&fit=crop', @CCat1, 499.99, 1),
('Sıfırdan İleri Seviye Flutter', 'https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=500&auto=format&fit=crop', @CCat2, 349.50, 1),
('React Native ile Mobil Uygulama', 'https://images.unsplash.com/photo-1551650975-87deedd944c3?w=500&auto=format&fit=crop', @CCat2, 299.00, 1);

INSERT INTO Testimonials (Name, Title, Comment, ImageUrl, Star) VALUES 
('Ayşe Kaya', 'Mobil Geliştirici', 'Bu platform sayesinde ilk uygulamamı markete yükledim!', 'https://randomuser.me/api/portraits/women/44.jpg', 5),
('Mehmet Öz', 'Veri Analisti', 'Kurs içerikleri çok güncel ve anlaşılır.', 'https://randomuser.me/api/portraits/men/45.jpg', 4),
('Fatma Demir', 'Öğrenci', 'Ücretsiz kurslar bile harika bir temel atmanızı sağlıyor.', 'https://randomuser.me/api/portraits/women/46.jpg', 5);
