using System;

namespace CardStorageService.Models.Requests
{
    // Создать запрос для добавления новой карты 
    public class CreateCardRequest
    {
        public int ClientId { get; set; }
        public string CardNo { get; set; }
        public string? Name { get; set; }
        public string? CVV2 { get; set; }
        public DateTime ExDate { get; set; } // истечение срока
    }
}
