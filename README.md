# 🍽️ Recipe Book Project

**Recipe Book Project**, kullanıcıların yemek tarifleri oluşturabileceği, malzemeleri yönetebileceği ve porsiyon/gramaj hesaplamaları yapabileceği kapsamlı bir web uygulamasıdır.

Bu proje, **.NET** ekosistemi üzerinde **N-Katmanlı Mimari (N-Layer Architecture)** prensipleri izlenerek geliştirilmiştir. Sürdürülebilir, test edilebilir ve modüler bir kod yapısına sahiptir.

## 🏗️ Mimari Yapı

Proje, sorumlulukların ayrılması (SoC) ilkesine uygun olarak aşağıdaki katmanlardan oluşmaktadır:

* **RecipeBookProject.WebApi:** Dış dünya ile iletişim kuran RESTful API katmanı.
* **RecipeBookProject.Business:** İş mantığının (validasyonlar, hesaplamalar) yürütüldüğü katman.
* **RecipeBookProject.DataAccess:** Veritabanı işlemlerinin (CRUD) soyutlandığı katman.
* **RecipeBookProject.Contracts:** Veri Transfer Objeleri (DTO) ve arayüzlerin bulunduğu katman.
* **RecipeBookProject.Client:** Kullanıcı arayüzü (Frontend) katmanı.
* **RecipeBookProject.Data:** Veritabanı modelleri (Entity) katmanı.

## ✨ Özellikler

* **Tarif Yönetimi:** Yeni tarif ekleme, düzenleme ve listeleme.
* **Malzeme & Gramaj:** Tarifler için malzeme miktarlarını ve gramajlarını ayarlama.
* **Modüler Yapı:** Kolay genişletilebilir servis mimarisi.
* **Veritabanı Entegrasyonu:** SQL Server tabanlı veri saklama.

## 🛠️ Teknolojiler

* **Framework:** .NET
* **Dil:** C#
* **Veritabanı:** MSSQL (Microsoft SQL Server)
* **ORM:** Entity Framework Core
* **Frontend:** HTML, CSS, JavaScript, Blazor

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin:

### Gereksinimler

* [.NET SDK](https://dotnet.microsoft.com/download)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (veya LocalDB)
* Visual Studio veya VS Code

### Adım Adım Kurulum

1.  **Projeyi Klonlayın:**
    ```bash
    git clone [https://github.com/muhammedaliderindag/RecipeBookProject.git](https://github.com/muhammedaliderindag/RecipeBookProject.git)
    cd RecipeBookProject
    ```

2.  **Veritabanını Hazırlayın:**
    * Proje dizininde bulunan `RecipeBookProject.bak` dosyasını SQL Server Management Studio (SSMS) kullanarak restore edin.
    * **Veya;** `appsettings.json` içerisindeki "ConnectionStrings" alanını kendi yerel veritabanınıza göre düzenleyip "Update-Database" komutunu (Code First kullanıldıysa) çalıştırın.

3.  **Bağımlılıkları Yükleyin:**
    ```bash
    dotnet restore
    ```

4.  **Projeyi Başlatın:**
    `RecipeBookProject.WebApi` (veya başlangıç projesi olarak belirlenen proje) dizinine gidin ve çalıştırın:
    ```bash
    dotnet run
    ```

## 📝 Lisans

Bu proje [MIT Lisansı](LICENSE.txt) ile lisanslanmıştır.

---

**Geliştirici:** [Muhammed Ali Derindağ](https://github.com/muhammedaliderindag)
