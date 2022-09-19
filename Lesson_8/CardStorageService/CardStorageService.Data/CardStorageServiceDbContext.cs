using Microsoft.EntityFrameworkCore;
using System;

namespace CardStorageService.Data
{
    public class CardStorageServiceDbContext : DbContext
    {            
        #region Контексты связи с таблицами БД
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Card> Cards { get; set; } // virtual необходим для переопределения свойств самим фреймворком (geter и seter).
                                                       // в рантайме. 
                                                       // в рантайме. 
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountSession> AccountSessions { get; set; }
        #endregion


        public CardStorageServiceDbContext(DbContextOptions options) : base(options)
        {

        }

        // DbContext.OnConfiguring Переопределите этот метод, чтобы настроить базу данных (и другие параметры),
        // которая будет использоваться для этого контекста.
        // Этот метод вызывается для каждого экземпляра создаваемого контекста. Базовая реализация ничего не делает.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer();
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
