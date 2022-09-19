using CardStorageService.Data;
using System;

namespace CardStorageService.Models
{
    // Data Transfer Object объект передачи данных (DTO). DTO — это объект, определяющий способ отправки данных по сети. Для того чтобы
    // не отправлять лишние данные создается буферный объект. чтобы не работать на прямую с базой данных
    // если добавить владельца карты (то есть циклическую ссылку то будет ошибка)
    public class CardDto
    {
        public Guid CardId { get; set; }


        public string CardNo { get; set; }


        public string? Name { get; set; }


        public string? CVV2 { get; set; }

        public string ExDate { get; set; }
        public int? ClientId { get; set; }

        // public virtual Client Client { get; set; } // отключено, так как вызовет ошибку циклической ссылки

    }
}
