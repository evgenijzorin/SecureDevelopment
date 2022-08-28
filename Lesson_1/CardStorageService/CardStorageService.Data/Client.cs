using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardStorageService.Data
{
    public class Client
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Указывает что в базе данных это поле будет идентификатором,
                                                                   // и будет генерироваться автоматически
        public int ClientId { get; set; }

        [Column]
        [StringLength(255)]
        public string? Surname { get; set; }

        [Column]
        [StringLength(255)]
        public string? FirstName { get; set; }

        [Column]
        [StringLength(255)]
        public string? Patronymic { get; set; }                       

        // Конструкция будет содержать все карты которыми владеет клиент
        [InverseProperty(nameof(Card.Client))] // взаимосвязь с полем ForeignKey. То есть задаст коллекцию <Card> в которых указаны значения
                                               // Card.Client с текущим ClientId. (Заданным атрибутом Key)
                                               // Атрибуты связи: InverseProperty и ForeignKey
        public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
    }
}
