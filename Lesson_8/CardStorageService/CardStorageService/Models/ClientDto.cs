using CardStorageService.Data;
using System.Collections.Generic;

namespace CardStorageService.Models
{
    public class ClientDto
    {
        public int ClientId { get; set; }

        public string? Surname { get; set; }

        public string? FirstName { get; set; }

        public string? Patronymic { get; set; }

        // public virtual ICollection<Card>? Cards { get; set; } = new HashSet<Card>();// отключено, так как вызовет ошибку циклической ссылки
    }
}
