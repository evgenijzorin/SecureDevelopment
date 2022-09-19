using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FullTextSearch.Services;
using FullTextSearch.Services.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace FullTextSearch
{
    internal class Sample02
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
               })
           .Build();

            var documentsSet = DocumentExtractor.DocumentsSet()
                .Take(10000) // Колличество забора данных
                .ToArray();
            // 1-й способ
            // new SimpleSearcher().Search("Monday", documentsSet);
            // 2-й способ
            // new SimpleSercherV2().SearchV1("Monday", documentsSet);
            // 3-й способ
            // new SimpleSercherV2().SearchV2("Monday", documentsSet);

            // Инструкция запуска Benchmark в режиме debug с умеренной потерей в точности.
            BenchmarkSwitcher.FromAssembly(typeof(Sample02).Assembly)
                .Run(args, new BenchmarkDotNet.Configs.DebugInProcessConfig());
            // Определить класс участвующий в расчете производительности
            BenchmarkRunner.Run<SearchBenchmarkV1>();

        }
    }

    #region Search test with benchmark
    [MemoryDiagnoser]
    [WarmupCount(1)]
    [IterationCount(5)]
    public class SearchBenchmarkV1
    {
        private readonly string[] _documentsSet;
        public SearchBenchmarkV1()
        {
            _documentsSet = DocumentExtractor.DocumentsSet().Take(10000).ToArray();
        }
        [Benchmark]
        public void SimpleSearch()
        {
            new SimpleSearcherV2().SearchV3("Monday", _documentsSet).ToArray();
        }

    }
    #endregion












}
