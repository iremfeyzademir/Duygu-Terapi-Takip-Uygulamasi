# Terapi Takip Uygulaması

Bu uygulama, günlük duygu durumu ve düşüncelerinizi kaydetmenize, analiz etmenize ve takip etmenize yardımcı olan bir C# Windows Forms uygulamasıdır.

## Kurulum

1. Projeyi bilgisayarınıza indirin
2. Visual Studio ile projeyi açın
3. Gerekli NuGet paketlerinin yüklenmesini bekleyin
   - System.Data.SQLite
   - System.Data.SQLite.Core

## Veritabanı Kurulumu

Uygulama ilk çalıştırıldığında otomatik olarak `terapi.db` adında bir SQLite veritabanı oluşturacaktır. Bu veritabanı aşağıdaki özelliklere sahiptir:

- Varsayılan kullanıcı bilgileri:
  - Kullanıcı adı: `admin`
  - Şifre: `admin123`

## Özellikler

- Kullanıcı kaydı ve girişi
- Günlük duygu durumu ve düşünce kayıtları
- Otomatik duygu analizi
- Kişiselleştirilmiş öneriler
- Geçmiş kayıtları görüntüleme ve düzenleme
- İstatistikler ve raporlar

## Kullanım

1. Uygulamayı başlatın
2. Varsayılan kullanıcı bilgileriyle giriş yapın veya yeni bir hesap oluşturun
3. Ana menüden istediğiniz işlemi seçin:
   - Yeni günlük girişi
   - Geçmiş girişler
   - İstatistikler

## Veritabanı Yapısı

Uygulama iki ana tablo kullanır:

1. Users Tablosu:
   - Id (PRIMARY KEY)
   - Username
   - Password

2. Journal Tablosu:
   - Id (PRIMARY KEY)
   - UserId (FOREIGN KEY)
   - Date
   - Mood
   - Content
   - Sentiment
   - Analysis
   - Suggestions

## Notlar

- Veritabanı dosyası (`terapi.db`) uygulama klasöründe otomatik olarak oluşturulur
- Eğer veritabanı dosyası bulunamazsa, uygulama otomatik olarak yeni bir veritabanı oluşturur
- Tüm veriler SQLite veritabanında yerel olarak saklanır

## Güvenlik

- Şifrelerin güvenliği için gerçek bir uygulamada şifreleme kullanılması önerilir
- Veritabanı dosyasını güvenli bir konumda saklayın
- Hassas verileri paylaşmadan önce veritabanını temizleyin 