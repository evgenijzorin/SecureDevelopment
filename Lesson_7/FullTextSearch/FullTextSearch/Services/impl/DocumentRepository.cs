using FullTextSearch.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch.Services.impl
{
    public class DocumentRepository : IDocumentRepository
    {
        #region Services
        private readonly DocumentDbContext _dbContext;
        #endregion

        #region Constructors
        public DocumentRepository(DocumentDbContext documentDbContext)
        {
            _dbContext = documentDbContext;
        }
        #endregion

        /// <summary>
        /// Метод (академичесеий пример) для получения статьи из документа по табуляции и номеру
        /// </summary>
        public void LoadDocuments()
        {
            
            // Создать StreamReader - Реализует объект TextReader, который считывает символы из потока байтов в определенной кодировке.
            using (var streamReader = new StreamReader(AppContext.BaseDirectory + "sample\\" + "data.txt"))
            {
                while (!streamReader.EndOfStream)
                {
                    // Прочитать строку и разделить по табуляции
                    var doc = streamReader.ReadLine().Split('\t'); // ReadLine Выполняет чтение строки символов из текущего потока и возвращает данные в виде строки.
                    if (doc.Length > 1
                        && int.TryParse(doc[0], out int id)) // Перевести первый элемент в int                  
                    {
                        _dbContext.Documents.Add(new Document
                        {
                            Id = id,
                            Content = doc[1]
                        });
                        _dbContext.SaveChanges();
                    }
                }

            }
        }



    }
}
