using CardStorageService.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardStorageService.Services.Impl
{
    public class ClientRepository : IClientRepositoryService
    {
        #region Services
        private readonly CardStorageServiceDbContext _dbContext; // Ссылка на контекст
        private readonly ILogger<ClientRepository> _logger; // Ведение журнала
        private readonly IOptions<DatabaseOptions> _databaseOptions; // Переменная опций для сохранения опций
        #endregion

        // В конструкторе класса инициализируем сервисы (DI)
        public ClientRepository(
            ILogger<ClientRepository> logger,
            IOptions<DatabaseOptions> databaseOptions,
            CardStorageServiceDbContext _dbContex)
        {
            _logger = logger;
            _dbContext = _dbContex;
            _databaseOptions = databaseOptions;
        }
        public int Create(Client data)
        {
            _dbContext.Clients.Add(data); // добавить в базу данных клиента
            _dbContext.SaveChanges(); // сохранить изменения в базе данных
            return data.ClientId; // вернуть Id клиента в случае удачного добавления
        }
        public IList<Client> GetAll()
        {            
            return _dbContext.Clients.ToList();
        }

        public Client GetById(int id)
        {
            return _dbContext.Clients.FirstOrDefault(client => client.ClientId == id);
        }
        public Client GetByCardId(string id)
        {
            var client = new Client();
            Card card = new Card();
            var clients = _dbContext.Clients.ToList();
            for (int i = 0; i<clients.Count; i++)
            {
                client = clients[i];
                card = client.Cards.FirstOrDefault(card => card.CardId.ToString() == id);
                if (card != null)
                    return client;
            }

            if(card == null)
            {
                throw new Exception("Client not found");               
            }
            return null;
        }

        public int Update(Client data)
        {
            _dbContext.Update(data);
            _dbContext.SaveChanges();
            return 1;
        }
        public int Delete(int id)
        {
            var client = _dbContext.Clients.FirstOrDefault  
                (client => client.ClientId == id);
            if (client == null)
            {
                throw new Exception("Client not found");
            }
            _dbContext.Remove(client);
            _dbContext.SaveChanges();
            return 1;
        }
    }
}
