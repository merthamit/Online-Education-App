# 🎓 OnlineEdu — Online Eğitim Platformu

ASP.NET Core 8 ile geliştirilmiş, **N-Katmanlı mimaride** gevşek bağlı bir online eğitim platformu.
Web API + MVC arayüzünden oluşur; **Redis** dağıtık önbellek, **RabbitMQ** asenkron mesajlaşma, **Cloudinary CDN** medya dağıtımı ve **Docker** ile konteynerize edilmiş bir geliştirme ortamı içerir.

**Kullanılan teknolojiler:** C# · .NET 8 · ASP.NET Core Web API & MVC · Entity Framework Core · SQL Server · Microsoft Identity · JWT · Redis · RabbitMQ (MassTransit) · Cloudinary CDN · Docker

---

## 🏗️ Mimari

```
┌──────────────┐   HTTP + JWT   ┌──────────────────────────────────────────────┐
│ OnlineEdu.   │ ─────────────▶ │ OnlineEduAPI (ASP.NET Core Web API)          │
│ WebUI (MVC)  │                │  ├─ OnlineEdu.Business   (servis/manager)    │
└──────────────┘                │  ├─ OnlineEdu.DataAccess (EF Core, repository)│
                                │  ├─ OnlineEdu.Entity     (varlıklar)          │
                                │  └─ OnlineEdu.DTO        (veri transfer)      │
                                └───────┬──────────┬──────────┬────────────────┘
                                        │          │          │
                                  SQL Server     Redis     RabbitMQ ──▶ SendEmailConsumer
                                                              Cloudinary CDN (görseller)
```

| Katman | Görev |
|---|---|
| **Entity** | Veritabanı varlıkları (Course, Blog, CourseReview, TeacherApplication…) |
| **DataAccess** | EF Core `DbContext`, generic repository, migration'lar |
| **Business** | Generic manager/servisler, Redis önbellekleme, Cloudinary servisi |
| **DTO** | API ile istemci arasındaki veri modelleri |
| **OnlineEduAPI** | REST API, JWT doğrulama, Identity, MassTransit (RabbitMQ) |
| **OnlineEdu.WebUI** | MVC arayüzü: site, Admin / Eğitmen / Öğrenci panelleri |

---

## ⚡ Altyapı Kullanımı

### Redis — dağıtık önbellek
- Kurs listesi ve kurs kategorileri
- Blog listeleri (son 4 blog dahil)
- Kurs puan özetleri (ortalama puan + değerlendirme sayısı)
- Herkese açık istatistikler (5 dk)
- Ekleme / güncelleme / silme işlemlerinde ilgili önbellek otomatik temizlenir

### RabbitMQ — asenkron olaylar (MassTransit)
`SendEmailCommand` olayları kuyruğa atılır, `SendEmailConsumer` arka planda işler:
- Yeni üyeye hoş geldin e-postası
- Kursa kayıt onay e-postası
- İletişim formu mesajı alındı bildirimi
- Yeni eğitmenlik başvurusunda yöneticilere bildirim
- Başvuru onay / ret sonucu e-postası

### Cloudinary — CDN
- Kurs, blog ve profil fotoğrafları Cloudinary'ye yüklenir ve CDN adresiyle sunulur
- Kayıt silindiğinde veya görsel değiştirildiğinde eski görsel Cloudinary'den de silinir

---

## 🌟 Özellikler

**Ziyaretçi & Öğrenci**
- Kayıt / giriş (Identity + JWT), canlı şifre kuralları, Türkçe hata mesajları
- Kurs listesi: arama, kategori, fiyat, **puan filtresi**, sıralama — sayfa yenilenmeden (AJAX)
- Kurs detayı, kursa kayıt, **Kayıt Olduğum Kurslar** sayfası, kurs videoları
- Kurslara **1–5 yıldız puan ve yorum** (sadece kayıtlı öğrenciler)
- **Eğitmenlik başvurusu** ve başvuru durumu takibi
- Blog, blog arama, yorum (sadece üyeler), önceki/sonraki yazı
- Eğitmen sayfaları (eğitmenin kursları), profil ayarları ve **profil fotoğrafı yükleme**

**Eğitmen Paneli**
- Kurs açma (sürükle-bırak görsel yükleme), kurs videoları, blog yazıları, sosyal medya hesapları
- Kendi eğitmen sayfasında "Kurs Aç" ve onay bekleyen kurs bilgisi

**Admin Paneli ("Mert Admin")**
- Dashboard: istatistik kartları, son kurslar ve mesajlar, bekleyen başvuru uyarısı
- Tüm içerik için CRUD: kurs, kategori, blog, banner, hakkımızda, referans, sosyal medya…
- Kullanıcı & rol yönetimi, **eğitmenlik başvurularını onaylama / reddetme**
- Panel içi arama paleti (`/` kısayolu), hızlı ekleme menüsü, breadcrumb

**Güvenlik**
- Rol tabanlı yetkilendirme (Admin / Teacher / Student), JWT ile korunan API
- Kullanıcılar yalnızca kendi profillerini görüntüleyip güncelleyebilir
- Kimlik bilgileri (şifre hash'i vb.) API yanıtlarında dışarı verilmez
- Gizli ayarlar koddan ayrı tutulur (`.env`)

---

## 🐳 Kurulum ve Çalıştırma

**Gereksinimler:** Docker Desktop

```bash
git clone <repo-adresi>
cd Online-Education-App-main
cp .env.example .env      # .env içine kendi Cloudinary bilgilerinizi ve şifrelerinizi girin
docker compose up -d --build
```

| Servis | Adres |
|---|---|
| Web arayüzü | http://localhost:5001 |
| API (Swagger) | http://localhost:5000/swagger |
| RabbitMQ yönetim paneli | http://localhost:15672 (guest / guest) |
| Redis | localhost:6379 |
| SQL Server | localhost,1433 |

API ilk açılışta migration'ları uygular, rolleri ve admin hesabını oluşturur.

### Demo verisi (isteğe bağlı)
Siteyi eğitmen, öğrenci, kurs, blog, değerlendirme vb. örnek verilerle doldurmak için:

```bash
pip install requests
python scripts/seed_demo_data.py
```

### Demo hesapları
| Rol | E-posta | Şifre |
|---|---|---|
| Admin | admin@admin.com | Admin123! |
| Eğitmen | ogretmen@test.com | Test123!* |
| Öğrenci | ogrenci@test.com | Test123!* |

> Eğitmen ve öğrenci hesapları demo verisi betiğiyle oluşturulur. Bu hesaplar yalnızca yerel geliştirme içindir.

---

## 📁 Proje Yapısı

```
OnlineEdu.Entity/        Varlıklar
OnlineEdu.DataAccess/    DbContext, repository'ler, migration'lar
OnlineEdu.Business/      Servisler, Redis önbellekleme, Cloudinary
OnlineEdu.DTO/           DTO'lar
OnlineEduAPI/            Web API, controller'lar, RabbitMQ consumer
OnlineEdu.WebUI/         MVC arayüzü ve paneller
scripts/                 Demo verisi betiği
docker-compose.yml       SQL Server, Redis, RabbitMQ, API, WebUI
```
