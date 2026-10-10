"""
OnlineEdu demo verisi yükleyici.

Siteyi gerçekçi verilerle doldurur: eğitmenler, öğrenciler, kategoriler, kurslar,
kurs videoları, bloglar, yorumlar, referanslar, sosyal medya, aboneler ve mesajlar.
Tekrar çalıştırılabilir; var olan kayıtları atlar.

Kullanım (docker compose up -d sonrası):
    pip install requests
    python scripts/seed_demo_data.py

Tüm demo kullanıcıların şifresi: DEMO_PASSWORD
Admin: admin@admin.com / Admin123!
"""
import datetime
import random
import requests

API = "http://localhost:5000/api"
ADMIN = ("admin@admin.com", "Admin123!")
DEMO_PASSWORD = "Test123!*"
random.seed(42)


def token(email, password):
    r = requests.post(f"{API}/Users/login", json={"email": email, "password": password})
    r.raise_for_status()
    return r.json()["token"]


def auth(tok):
    return {"Authorization": f"Bearer {tok}"}


admin_h = auth(token(*ADMIN))


def get(path, headers=None):
    r = requests.get(f"{API}/{path}", headers=headers or admin_h)
    r.raise_for_status()
    return r.json()


def post(path, data, headers=None):
    r = requests.post(f"{API}/{path}", json=data, headers=headers or admin_h)
    if not r.ok and r.status_code != 409:
        print(f"  ! {path}: {r.status_code} {r.text[:150]}")
    return r


def put(path, data, headers=None):
    r = requests.put(f"{API}/{path}", json=data, headers=headers or admin_h)
    if not r.ok:
        print(f"  ! PUT {path}: {r.status_code} {r.text[:150]}")
    return r


# ---------------------------------------------------------------- kullanıcılar
TEACHERS = [
    ("Emre", "Aydın", "ogretmen1", "ogretmen@test.com", "https://randomuser.me/api/portraits/men/32.jpg"),
    ("Elif", "Şahin", "elifsahin", "elif.sahin@onlineedu.com", "https://randomuser.me/api/portraits/women/44.jpg"),
    ("Burak", "Demir", "burakdemir", "burak.demir@onlineedu.com", "https://randomuser.me/api/portraits/men/46.jpg"),
    ("Selin", "Arslan", "selinarslan", "selin.arslan@onlineedu.com", "https://randomuser.me/api/portraits/women/65.jpg"),
    ("Can", "Öztürk", "canozturk", "can.ozturk@onlineedu.com", "https://randomuser.me/api/portraits/men/75.jpg"),
    ("Deniz", "Koç", "denizkoc", "deniz.koc@onlineedu.com", "https://randomuser.me/api/portraits/women/68.jpg"),
]
STUDENTS = [
    ("Zeynep", "Kaya", "ogrenci1", "ogrenci@test.com", "https://randomuser.me/api/portraits/women/12.jpg"),
    ("Mehmet", "Yıldız", "mehmetyildiz", "mehmet.yildiz@mail.com", "https://randomuser.me/api/portraits/men/11.jpg"),
    ("Ayşe", "Çelik", "aysecelik", "ayse.celik@mail.com", "https://randomuser.me/api/portraits/women/21.jpg"),
    ("Ali", "Kurt", "alikurt", "ali.kurt@mail.com", "https://randomuser.me/api/portraits/men/22.jpg"),
    ("Fatma", "Aksoy", "fatmaaksoy", "fatma.aksoy@mail.com", "https://randomuser.me/api/portraits/women/33.jpg"),
    ("Mustafa", "Polat", "mustafapolat", "mustafa.polat@mail.com", "https://randomuser.me/api/portraits/men/34.jpg"),
    ("Merve", "Erdoğan", "merveerdogan", "merve.erdogan@mail.com", "https://randomuser.me/api/portraits/women/45.jpg"),
    ("Hakan", "Güneş", "hakangunes", "hakan.gunes@mail.com", "https://randomuser.me/api/portraits/men/56.jpg"),
    ("Esra", "Doğan", "esradogan", "esra.dogan@mail.com", "https://randomuser.me/api/portraits/women/57.jpg"),
    ("Oğuz", "Tekin", "oguztekin", "oguz.tekin@mail.com", "https://randomuser.me/api/portraits/men/67.jpg"),
]


def ensure_user(first, last, username, email, image, role):
    users = get("RoleAssigns")
    u = next((x for x in users if x["userName"] == username), None)
    if u is None:
        post("Users/register", {"firstName": first, "lastName": last, "userName": username, "email": email,
                                "password": DEMO_PASSWORD, "confirmPassword": DEMO_PASSWORD}, headers={})
        u = next(x for x in get("RoleAssigns") if x["userName"] == username)
    uid = u["id"]
    if role not in u["roles"]:
        roles = get(f"RoleAssigns/{uid}")
        for r in roles:
            r["roleExist"] = r["roleName"] == role
        post("RoleAssigns", roles)
    profile = get(f"Users/GetMyProfile/{uid}")
    put("Users/UpdateMyProfile", {"id": uid, "firstName": first, "lastName": last,
                                  "email": profile["email"], "imageUrl": image})
    return uid


print("Kullanıcılar...")
teacher_ids = [ensure_user(*t, role="Teacher") for t in TEACHERS]
student_ids = [ensure_user(*s, role="Student") for s in STUDENTS]

# ---------------------------------------------------------------- eğitmen sosyal medya
print("Eğitmen sosyal medya hesapları...")
existing_socials = get("TeacherSocials")
for tid, (first, last, username, *_ ) in zip(teacher_ids, TEACHERS):
    if any(s["teacherId"] == tid for s in existing_socials):
        continue
    for name, icon, url in [("LinkedIn", "fab fa-linkedin-in", f"https://www.linkedin.com/in/{username}"),
                            ("GitHub", "fab fa-github", f"https://github.com/{username}"),
                            ("Twitter", "fab fa-twitter", f"https://twitter.com/{username}")]:
        post("TeacherSocials", {"url": url, "socialMediaName": name, "icon": icon, "teacherId": tid})

# ---------------------------------------------------------------- kurs kategorileri
print("Kurs kategorileri...")
CATEGORIES = [
    ("Yazılım Geliştirme", "fas fa-code", "Web, mobil ve masaüstü uygulama geliştirme"),
    ("Tasarım", "fas fa-paint-brush", "UI/UX, grafik ve ürün tasarımı"),
    ("İş Dünyası", "fas fa-briefcase", "Girişimcilik, yönetim ve finans"),
    ("Veri Bilimi", "fas fa-database", "Veri analizi, makine öğrenmesi ve yapay zeka"),
    ("Mobil Geliştirme", "fas fa-mobile-alt", "Android, iOS ve çapraz platform"),
    ("Siber Güvenlik", "fas fa-shield-alt", "Ağ güvenliği, etik hacking ve savunma"),
]
cats = {c["categoryName"]: c["courseCategoryId"] for c in get("CourseCategories")}
for name, icon, desc in CATEGORIES:
    if name not in cats:
        post("CourseCategories", {"categoryName": name, "icon": icon, "description": desc, "isShown": True})
cats = {c["categoryName"]: c["courseCategoryId"] for c in get("CourseCategories")}

# ---------------------------------------------------------------- kurslar
print("Kurslar...")
U = "https://images.unsplash.com/photo-{}?w=900&auto=format&fit=crop"
COURSES = [
    # ad, kategori, fiyat, eğitmen index, görsel, videolar (YouTube id)
    ("Sıfırdan C# ve .NET Core", "Yazılım Geliştirme", 299.99, 0, U.format("1550439062-609e1531270e"), ["GhQdlIFylQ8"]),
    ("Modern Web Geliştirme (React & Node.js)", "Yazılım Geliştirme", 349.99, 1, U.format("1633356122544-f134324a6cee"), ["bMknfKXIFA8", "PkZNo7MFNFg"]),
    ("ASP.NET Core 8 ile N-Katmanlı Mimari", "Yazılım Geliştirme", 449.00, 0, None, ["GhQdlIFylQ8", "fqMOX6JJhGo"]),
    ("Sıfırdan JavaScript", "Yazılım Geliştirme", 199.00, 1, U.format("1579468118864-1b9ea3c0db4a"), ["PkZNo7MFNFg"]),
    ("Docker ve Konteyner Teknolojileri", "Yazılım Geliştirme", 279.00, 2, U.format("1605745341112-85968b19335b"), ["fqMOX6JJhGo"]),
    ("Git ve GitHub ile Versiyon Kontrolü", "Yazılım Geliştirme", 0, 2, U.format("1556075798-4825dfaaf498"), ["RGOj5yH7evk"]),
    ("Python ile Veri Bilimine Giriş", "Veri Bilimi", 399.00, 3, U.format("1551288049-bebda4e38f71"), ["rfscVS0vtbw"]),
    ("Makine Öğrenmesi Temelleri", "Veri Bilimi", 499.00, 3, U.format("1555949963-aa79dcee981c"), ["NWONeJKn6kc", "rfscVS0vtbw"]),
    ("SQL ile Veritabanı Yönetimi", "Veri Bilimi", 249.00, 4, U.format("1544383835-bda2bc66a55d"), ["HXV3zeQKqGY"]),
    ("Figma ile UI/UX Tasarımı", "Tasarım", 329.00, 5, U.format("1561070791-2526d30994b5"), []),
    ("Sıfırdan Flutter ile Mobil Uygulama", "Mobil Geliştirme", 379.00, 4, U.format("1512941937669-90a1b58e7e9c"), []),
    ("Etik Hacking ve Siber Güvenlik", "Siber Güvenlik", 459.00, 2, U.format("1550751827-4bd374c3f58b"), []),
    ("Dijital Pazarlama ve Girişimcilik", "İş Dünyası", 219.00, 5, U.format("1460925895917-afdab827c52f"), []),
]
existing = {c["courseName"]: c for c in get("Courses")}
for name, cat, price, t_idx, img, _ in COURSES:
    if name in existing:
        c = existing[name]
        put("Courses", {"courseId": c["courseId"], "courseName": name, "imageUrl": c["imageUrl"],
                        "courseCategoryId": c["courseCategoryId"], "price": c["price"], "isShown": True,
                        "appUserId": teacher_ids[t_idx]})
    else:
        post("Courses", {"courseName": name, "imageUrl": img, "courseCategoryId": cats[cat], "price": price,
                         "isShown": True, "appUserId": teacher_ids[t_idx]})
courses = {c["courseName"]: c["courseId"] for c in get("Courses")}

print("Kurs videoları...")
for name, *_, videos in COURSES:
    cid = courses[name]
    have = get(f"CourseVideos/GetCourseVideosById/{cid}")
    if have:
        continue
    for i, vid in enumerate(videos, start=1):
        post("CourseVideos", {"videoNumber": i, "courseId": cid, "videoUrl": f"https://www.youtube.com/embed/{vid}"})

# ---------------------------------------------------------------- öğrenci kayıtları
print("Öğrenci kurs kayıtları...")
course_ids = list(courses.values())
for (first, last, username, email, _), sid in zip(STUDENTS, student_ids):
    h = auth(token(email, DEMO_PASSWORD))
    for cid in random.sample(course_ids, k=min(3, len(course_ids))):
        post("CourseRegisters", {"appUserId": sid, "courseId": cid}, headers=h)

# ---------------------------------------------------------------- bloglar
print("Blog kategorileri ve bloglar...")
blog_cats = {c["name"]: c["blogCategoryId"] for c in get("BlogCategories")}
for name in ["Yazılım Teknolojileri", "Kariyer Tavsiyeleri", "Yapay Zeka", "Siber Güvenlik", "Tasarım"]:
    if name not in blog_cats:
        post("BlogCategories", {"name": name})
blog_cats = {c["name"]: c["blogCategoryId"] for c in get("BlogCategories")}

BLOGS = [
    ("Yapay Zeka Yazılımcıların Yerini Alacak mı?", "Yapay Zeka", "1677442136019-21780ecad995",
     "Yapay zeka araçları kod yazma sürecini hızlandırıyor; ancak problem çözme, mimari tasarım ve iş analizi hâlâ insan uzmanlığı gerektiriyor. Bu yazıda yapay zekayı bir rakip değil, verimlilik aracı olarak nasıl kullanabileceğinizi anlatıyoruz."),
    ("2026'da Siber Güvenliğin Önemi", "Siber Güvenlik", "1550751827-4bd374c3f58b",
     "Her yıl artan siber saldırılar şirketleri güvenlik yatırımlarını artırmaya zorluyor. Parola yönetimi, iki adımlı doğrulama ve güncel yazılım kullanımı gibi temel önlemlerle risklerin büyük bölümünü azaltmak mümkün."),
    ("Web Geliştirmede Yeni Trendler", "Yazılım Teknolojileri", "1498050108023-c5249f4df085",
     "Frontend dünyası hızla değişiyor. Sunucu taraflı render, edge fonksiyonları ve bileşen tabanlı mimariler modern web uygulamalarının temelini oluşturuyor."),
    ("Redis ile Uygulamanızı Hızlandırın", "Yazılım Teknolojileri", "1558494949-ef010cbdcc31",
     "Sık tekrarlanan veritabanı sorgularını Redis gibi dağıtık bir önbellekte tutmak, yanıt sürelerini saniyelerden milisaniyelere indirebilir. Önbellek geçersiz kılma stratejileri ise verinin güncel kalmasını sağlar."),
    ("Junior Geliştiriciler İçin Kariyer Tavsiyeleri", "Kariyer Tavsiyeleri", "1522202176988-66273c2fd55f",
     "İlk işinizi bulmak için güçlü bir portföy, GitHub'da düzenli projeler ve temel bilgisayar bilimi kavramlarına hâkimiyet büyük fark yaratır. Mülakatlarda düşünce sürecinizi sesli anlatmayı unutmayın."),
    ("Uzaktan Çalışırken Verimli Kalmanın Yolları", "Kariyer Tavsiyeleri", "1587614382346-4ec70e388b28",
     "Sabit bir çalışma düzeni, ayrı bir çalışma alanı ve düzenli molalar uzaktan çalışmada odaklanmayı kolaylaştırır. Ekip içi iletişimde yazılı ve açık olmak da verimliliği artırır."),
    ("RabbitMQ ile Asenkron Mimariler", "Yazılım Teknolojileri", "1518432031352-d6fc5c10da5a",
     "Mesaj kuyrukları, servisler arasındaki bağımlılığı azaltarak sistemleri daha dayanıklı hale getirir. E-posta gönderimi gibi uzun süren işleri kuyruğa atmak, kullanıcıya anında yanıt vermenizi sağlar."),
    ("İyi Bir Arayüz Tasarımının 5 Kuralı", "Tasarım", "1561070791-2526d30994b5",
     "Tutarlılık, görsel hiyerarşi, yeterli boşluk, erişilebilir renk kontrastı ve net geri bildirim; kullanıcıların arayüzünüzü sezgisel bulmasını sağlayan temel ilkelerdir."),
    ("Makine Öğrenmesine Nereden Başlamalı?", "Yapay Zeka", "1555949963-aa79dcee981c",
     "Python, temel istatistik ve doğrusal cebir bilgisiyle başlayıp küçük veri setleri üzerinde denemeler yapmak, makine öğrenmesini öğrenmenin en etkili yoludur."),
    ("Bulut Bilişime Giriş: Docker'dan Kubernetes'e", "Yazılım Teknolojileri", "1605745341112-85968b19335b",
     "Konteynerler uygulamaları her ortamda aynı şekilde çalıştırmayı sağlar. Docker ile başlayıp ölçeklendirme ihtiyacı doğduğunda Kubernetes gibi orkestrasyon araçlarına geçebilirsiniz."),
]
existing_titles = {b["title"] for b in get("Blogs")}
now = datetime.datetime.now()
for i, (title, cat, img, content) in enumerate(BLOGS):
    if title in existing_titles:
        continue
    post("Blogs", {"title": title, "content": content, "imageUrl": U.format(img), "blogCategoryId": blog_cats[cat],
                   "writerId": teacher_ids[i % len(teacher_ids)], "blogDate": (now - datetime.timedelta(days=i + 1)).isoformat()})

print("Blog yorumları...")
COMMENTS = ["Çok faydalı bir yazı olmuş, teşekkürler!", "Tam aradığım bilgiler, elinize sağlık.",
            "Bu konuda bir kurs da açılsa harika olur.", "Örneklerle anlatılması çok iyi olmuş.",
            "Paylaştığınız kaynakları da inceleyeceğim, teşekkürler.", "Kısa ve net, çok beğendim."]
blogs = get("Blogs")
student_tokens = [auth(token(s[3], DEMO_PASSWORD)) for s in STUDENTS]
for b in blogs:
    if get(f"BlogComments/{b['blogId']}"):
        continue
    for h in random.sample(student_tokens, k=2):
        post("BlogComments", {"name": "-", "email": "-", "content": random.choice(COMMENTS), "blogId": b["blogId"]}, headers=h)

# ---------------------------------------------------------------- referanslar
print("Referanslar...")
TESTIMONIALS = [
    ("Ayşe Kaya", "Mobil Geliştirici", "Bu platform sayesinde ilk uygulamamı markete yükledim!", "https://randomuser.me/api/portraits/women/24.jpg", 5),
    ("Mehmet Öz", "Veri Analisti", "Kurs içerikleri çok güncel ve anlaşılır.", "https://randomuser.me/api/portraits/men/41.jpg", 5),
    ("Fatma Demir", "Üniversite Öğrencisi", "Eğitmenler sorulara çok hızlı dönüş yapıyor.", "https://randomuser.me/api/portraits/women/50.jpg", 4),
    ("Kerem Aslan", "Backend Geliştirici", "ASP.NET Core kursu ile işe girdim, herkese tavsiye ederim.", "https://randomuser.me/api/portraits/men/52.jpg", 5),
    ("Buse Yalçın", "UI/UX Tasarımcı", "Figma kursu sayesinde portföyümü oluşturdum.", "https://randomuser.me/api/portraits/women/60.jpg", 5),
]
names = {t["name"] for t in get("Testimonials")}
for n, title, comment, img, star in TESTIMONIALS:
    if n not in names:
        post("Testimonials", {"name": n, "title": title, "comment": comment, "imageUrl": img, "star": star})

# ---------------------------------------------------------------- banner
print("Banner...")
BANNER_IMG = "https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=1200&auto=format&fit=crop"
for b in get("Banners"):
    put("Banners", {"bannerId": b["bannerId"], "title": "Geleceğinize Bugün Yatırım Yapın", "imageUrl": BANNER_IMG})

# ---------------------------------------------------------------- sosyal medya
print("Sosyal medya...")
SOCIALS = [("Facebook", "fab fa-facebook-f", "https://facebook.com/onlineedu"),
           ("Twitter", "fab fa-twitter", "https://twitter.com/onlineedu"),
           ("Instagram", "fab fa-instagram", "https://instagram.com/onlineedu"),
           ("LinkedIn", "fab fa-linkedin-in", "https://linkedin.com/company/onlineedu"),
           ("YouTube", "fab fa-youtube", "https://youtube.com/@onlineedu")]
titles = {s["title"] for s in get("SocialMedias")}
for title, icon, url in SOCIALS:
    if title not in titles:
        post("SocialMedias", {"title": title, "icon": icon, "url": url})

# ---------------------------------------------------------------- aboneler ve mesajlar
print("Aboneler ve mesajlar...")
subs = {s["email"] for s in get("Subscribers")}
for s in STUDENTS[:8]:
    if s[3] not in subs:
        post("Subscribers", {"email": s[3]}, headers={})

MESSAGES = [
    ("Mehmet Yıldız", "mehmet.yildiz@mail.com", "Kurs sertifikası", "Kursları bitirdiğimde sertifika verilecek mi?"),
    ("Ayşe Çelik", "ayse.celik@mail.com", "Ödeme seçenekleri", "Taksitli ödeme imkânı var mı?"),
    ("Ali Kurt", "ali.kurt@mail.com", "Eğitmen başvurusu", "Platformunuzda eğitmen olmak istiyorum, nasıl başvurabilirim?"),
    ("Merve Erdoğan", "merve.erdogan@mail.com", "Video oynatma sorunu", "Bazı videolar mobilde açılmıyor, yardımcı olabilir misiniz?"),
    ("Hakan Güneş", "hakan.gunes@mail.com", "Kurumsal eğitim", "Şirketimiz için toplu eğitim paketi almak istiyoruz."),
]
subjects = {m["subject"] for m in get("Messages")}
for name, email, subject, content in MESSAGES:
    if subject not in subjects:
        post("Messages", {"name": name, "email": email, "subject": subject, "content": content}, headers={})


# ---------------------------------------------------------------- kurs değerlendirmeleri
print("Kurs değerlendirmeleri...")
REVIEW_TEXTS = {
    5: ["Harika bir kurs, anlatım çok net ve örnekler gerçek hayattan.", "Beklentimin çok üzerindeydi, kesinlikle tavsiye ederim.", "Eğitmen konuya çok hakim, her şey adım adım anlatılmış."],
    4: ["Gayet iyi bir kurs, bazı bölümler biraz daha detaylı olabilirdi.", "İçerik güncel ve faydalı, teşekkürler.", "Başlangıç için çok iyi bir kaynak."],
    3: ["Fena değil ama biraz hızlı ilerliyor.", "Temel konular iyi, ileri seviye için yetersiz kalabilir."],
}
for (first, last, username, email, _), sid in zip(STUDENTS, student_ids):
    h = auth(token(email, DEMO_PASSWORD))
    for reg in get(f"courseRegisters/GetMyCourses/{sid}", headers=h):
        if requests.get(f"{API}/CourseReviews/My/{reg['courseId']}", headers=h).status_code == 200:
            continue
        rating = random.choices([5, 4, 3], weights=[5, 4, 1])[0]
        post("CourseReviews", {"courseId": reg["courseId"], "rating": rating, "comment": random.choice(REVIEW_TEXTS[rating])}, headers=h)

# ---------------------------------------------------------------- örnek eğitmenlik başvurusu
print("Eğitmenlik başvurusu...")
applicant = STUDENTS[1]
h = auth(token(applicant[3], DEMO_PASSWORD))
if requests.get(f"{API}/TeacherApplications/My", headers=h).status_code == 204:
    post("TeacherApplications", {
        "expertise": "Backend Geliştirme (.NET, SQL)",
        "motivation": "5 yıldır .NET ile kurumsal projeler geliştiriyorum. Öğrendiklerimi uygulamalı kurslarla paylaşmak ve özellikle yeni başlayanlara temiz kod yazmayı öğretmek istiyorum.",
        "portfolioUrl": "https://github.com/mehmetyildiz"
    }, headers=h)

print("\nTamamlandı:")
print(f"  {len(get('Users/TeacherList'))} eğitmen, {len(get('Users/StudentList'))} öğrenci")
print(f"  {len(get('CourseCategories'))} kategori, {len(get('Courses'))} kurs, {len(get('Blogs'))} blog")
print(f"  {sum(x['count'] for x in get('CourseReviews/Summary'))} kurs değerlendirmesi, {get('TeacherApplications/PendingCount')} bekleyen eğitmenlik başvurusu")
print(f"  {len(get('Testimonials'))} referans, {len(get('SocialMedias'))} sosyal medya, {len(get('Subscribers'))} abone, {len(get('Messages'))} mesaj")
