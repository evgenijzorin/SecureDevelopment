using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardStorageService.Data
{
    [Table("Accounts")]
    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] // генерируется базой данных        
        public int AccountId { get; set; }
        [StringLength(255)]
        public string EMail { get; set; }
        [StringLength(100)]
        public string PasswordSalt { get; set; } // соль пароля 
        [StringLength(100)]
        public string PasswordHash { get; set; } // хэшь пароля
        public bool Locked { get; set; } // блокировка логина
        [StringLength(255)]
        public string FirstName {get; set; } 
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string SecondName { get; set; }

        [InverseProperty(nameof(AccountSession.Account))] // ссылка на сессии
        public virtual ICollection<AccountSession> AccountSessions { get; set; } = new HashSet<AccountSession>();
    }
}
