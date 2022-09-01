using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardStorageService.Data
{
    [Table("AccountSessions")]
    public class AccountSession
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] // генерируется базой данных
        public int SessionId { get; set; }
        [Required] // обязательный требуемый
        [StringLength(384)]
        public string SessionToken { get; set; } // сессионный ключ (токен пользователя)

        [ForeignKey(nameof(Account))] // ссылается на аккаунт
        public int AccountId { get; set; } // идентификатор пользователя
        [Column(TypeName = "datetime2")]
        public DateTime TimeCreated { get; set; } // время создания
        [Column(TypeName = "datetime2")]
        public DateTime TimeLastRequest { get; set; } // время последнего запроса
        public bool IsClosed { get; set; } // завершена ли сессия (logout)
        [Column(TypeName = "datetime2")]
        public DateTime? TimeClosed{ get; set; } // время завершения сессии
        



        public virtual Account Account { get; set; }

    }
}
