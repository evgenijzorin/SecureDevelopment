using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardStorageService.Data
{
    [Table("Cards")]
    public class Card
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] // задает атрибут для генерации свойства. при данной нотации свойство 
                                                                    // генерируется привставке строки таблицы
        public Guid CardId { get; set; } // Guid - уникальный идентификатор

        [ForeignKey(nameof(Client))] // ForeignKey (внешний ключ) атрибут указывает на зависимость от другой таблицы
                                     // в данном случае указывает на принадлежность конкретному клиенту (ссылку на Id клиента)

        public int ClientId { get; set; } 

        [Column]
        [StringLength(20)]
        public string CardNo { get; set; }

        [Column]
        [StringLength(50)]
        public string? Name { get; set; }

        [Column]
        [StringLength(50)]
        public string? CVV2 { get; set; } // card veryfication value (visa)/Card Verification Code (Mastercard)
                                          // card verification parameter (Мир)

        [Column]
        public DateTime ExDate { get; set; }
        public virtual Client Client { get; set; } // Поле заполняется автоматически entity frameworck - ом.
    }
}
