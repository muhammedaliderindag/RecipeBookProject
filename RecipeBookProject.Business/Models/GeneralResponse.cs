using RecipeBookProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class GeneralResponse<T>
    {
        // Dönen asıl veri. Generic tip T olarak belirlenmiştir.
        public T? Data { get; private set; }

        // İşlemin başarılı olup olmadığını belirten bayrak.
        public bool IsSuccess { get; private set; }

        // Kullanıcıya gösterilebilecek genel bir mesaj.
        public string? Message { get; private set; }

        // Hata durumunda oluşacak hata mesajlarının listesi.
        public List<string>? Errors { get; private set; }

        // HTTP durum kodunu temsil eder (200, 404, 500 vb.).
        // JSON'a serialize edilmemesi için JsonIgnore attribute'u ekledik.
        // Çünkü bu bilgi genellikle HTTP response header'ında zaten bulunur.
        // Ancak servisler arası iletişimde faydalı olabilir.
        [JsonIgnore]
        public int StatusCode { get; private set; }

        // Constructor'ı private yaparak nesne oluşturmanın sadece
        // static factory metotları (Success, Fail) ile yapılmasını sağlıyoruz.
        private GeneralResponse() { }


        // --- Static Factory Methods ---

        // Başarılı bir response oluşturmak için kullanılır (veri ile birlikte).
        public static GeneralResponse<T> Success(T data, string? message = null, int statusCode = 200)
        {
            return new GeneralResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        // Başarılı bir response oluşturmak için kullanılır (veri olmadan, sadece mesajla).
        // Örneğin bir silme işlemi sonrası.
        public static GeneralResponse<T> Success(string message, int statusCode = 200)
        {
            return new GeneralResponse<T>
            {
                Data = default, // T'nin varsayılan değeri (class için null, int için 0 vb.)
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        // Başarısız bir response oluşturmak için kullanılır (tek bir hata mesajı ile).
        public static GeneralResponse<T> Fail(string errorMessage, int statusCode = 400)
        {
            return new GeneralResponse<T>
            {
                IsSuccess = false,
                Errors = new List<string> { errorMessage },
                Message = errorMessage, // Genellikle ilk hata genel mesaj olarak atanabilir.
                StatusCode = statusCode
            };
        }

        // Başarısız bir response oluşturmak için kullanılır (hata listesi ile).
        // Özellikle validasyon hataları için kullanışlıdır.
        public static GeneralResponse<T> Fail(List<string> errors, int statusCode = 400)
        {
            return new GeneralResponse<T>
            {
                IsSuccess = false,
                Errors = errors,
                Message = errors.Count > 0 ? errors[0] : "An error occurred.",
                StatusCode = statusCode
            };
        }
    }
}
