using Microsoft.EntityFrameworkCore;
using System;

namespace CardStorageService.Data
{
    public class CardStorageServiceDbContext : DbContext
    {

        // Контексты связи с таблицами БД
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Card> Cards { get; set; } // virtual необходим для переопределения свойств самим фреймворком (geter и seter).
                                                       // в рантайме. 
                                                       // в рантайме. 
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountSession> AccountSessions { get; set; }


        public CardStorageServiceDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer();
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
