using FullTextSearch.Services;
using FullTextSearch.Services.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FullTextSearch
{
    internal class Sample01
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    #region Configure EF DbContext Service 
                    services.AddDbContext<DocumentDbContext>(option =>
                    {
                        option.UseSqlServer(@"data source = DESKTOP-E4UTNHO\SQLEXPRESS; initial catalog = DocumentsDatabase; User Id = DocumentsDatabaseUser; Password = 12345; MultipleActiveResultSets = True; App = EntityFramework");
                        // data source - имя компьютера
                        // initial catalog - имя базы данных
                        // User Id  - служебный пользователь базы данных
                        // MultipleActiveResultSets - несколько активных результирующих набора. Если указано значение true, приложение может поддерживать несколько
                        // активных наборов результатов (режим MARS). 
                    });
                    #endregion

                    #region Configure Repositories
                    services.AddTransient<IDocumentRepository, DocumentRepository>(); // Transient - сервис создается каждый раз когда его запрашивают
                    #endregion

                })
            .Build();

            // Вызываем метод поиска статьи  и сохраняем результат в БД
            host.Services
                .GetRequiredService<IDocumentRepository>() // Обратиться к сервису               
                .LoadDocuments();// Вызвать метод сервиса.
        }
    }
}
