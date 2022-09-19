using CardStorageService.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardStorageService.Services.Impl
{
    public class CardRepository : ICardRepositoryService
    {
        #region Services
        private readonly CardStorageServiceDbContext _dbContext; // Ссылка на контекст
        private readonly ILogger<CardRepository> _logger; // Ведение журнала
        private readonly IOptions<DatabaseOptions> _databaseOptions; // Переменная опций для сохранения опций
        #endregion

        // В конструкторе класса инициализируем сервисы (DI)
        public CardRepository(
            ILogger<CardRepository> logger,
            IOptions<DatabaseOptions> databaseOptions,
            CardStorageServiceDbContext _dbContex)
        {
            _logger = logger;
            _dbContext = _dbContex;
            _databaseOptions = databaseOptions;
        }

        public string Create(Card data)
        {
            var client = _dbContext.Clients.FirstOrDefault
                (client => client.ClientId == data.ClientId); //Возвращает первый элемент последовательности, удовлетворяющий
                                                              //указанному условию, или значение по умолчанию, если ни одного такого
                                                              //элемента не найдено.Значением по умолчанию для ссылочных и допускающих
                                                              //значение NULL типов является null.

            if (client == null)
                throw new Exception("Client not found");
            _dbContext.Cards.Add(data); // добавить в базу данных клиента
            _dbContext.SaveChanges(); // сохранить изменения в базе данных
            return data.CardId.ToString(); // вернуть Id клиента в случае удачного добавления
        }
        public IList<Card> GetAll()
        {
            // Выражение возвратит карты со свойством card.ClientI равным clientId
            return _dbContext.Cards.ToList();
        }
        public Card GetById(string id)
        {
            return _dbContext.Cards.FirstOrDefault(card => card.CardId.ToString() == id);
        }
        public IList<Card> GetByClientId(int clientId)
        {
            // Выражение возвратит карты со свойством card.ClientI равным clientId
            return _dbContext.Cards.Where(card => card.ClientId == clientId).ToList();  //Предложение where используется в выражении
                                                                                        //запроса для того, чтобы указать, какие элементы
                                                                                        //из источника данных будут возвращаться в выражении
                                                                                        //запроса.
            #region Альтернативный способ реализации метода. Заключается в обращеннии к базе данных не через контекст а через класс SqlConnection
            // (напрамую)
            //List<Card> cards = new List<Card>(); // переменная для хранения возвращаемого значения    
            //using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString)) // получаем sqlConnectin с заданными
            //                                                                                                 // опциями (которые содержат строку соединения)
            //{
            //    sqlConnection.Open(); // открыть базу данных с опциями соединения
            //    using (var sqlCommand = new SqlCommand(String.Format("select * from cards where ClientId = {0}", clientId.ToString()), sqlConnection))
            //                                                                                          // Составление команды запроса к базе данных на языке sql
            //    {
            //        var reader = sqlCommand.ExecuteReader(); // Получть ридер базы данных                    
            //        while (reader.Read()) // Переместь ридер на новую позицию. Счетчика итераций почемуто не нашел.
            //        {
            //            cards.Add(new Card
            //            {
            //                CardId = new Guid(reader["CardId"].ToString()),
            //                CardNo = reader["CardNo"]?.ToString(),
            //                Name = reader["Name"]?.ToString(),
            //                CVV2 = reader["CVV2"]?.ToString(),
            //                ExDate = Convert.ToDateTime(reader["ExDate"])
            //            });
            //        }
            //    }
            //}
            //return cards;
            #endregion
        }
        public int Update(Card data)
        {
            _dbContext.Update(data);
            _dbContext.SaveChanges();
            return 1;
        }

        public int Delete(string id)
        {
            var card = _dbContext.Cards.FirstOrDefault
               (card => card.CardId.ToString() == id);
            if (card == null)
            {
                throw new Exception("Card not found");                
            }               
            _dbContext.Remove(card);
            _dbContext.SaveChanges();
            return 1;
        }
    }
}
