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
    internal class Sample03
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

            // FullTextIndexV1 fullTextIndexV1 = new FullTextIndexV1(host.Services.GetService<DocumentDbContext>()); // передать контекст базы данных
            // fullTextIndexV1.BuildIndex();


            // Инструкция запуска Benchmark в режиме debug с умеренной потерей в точности.
            BenchmarkSwitcher.FromAssembly(typeof(Sample03).Assembly)
                .Run(args, new BenchmarkDotNet.Configs.DebugInProcessConfig());
            // Определить класс участвующий в расчете производительности
            BenchmarkRunner.Run<SearchBenchmarkV2>();
        }
    }

    [MemoryDiagnoser]
    [WarmupCount(1)]
    [IterationCount(5)]
    public class SearchBenchmarkV2
    {

        private readonly FullTextIndexV3 _index;
        private readonly string[] _documentsSet;

        // Атрибут запустит тесты для каждого слова
        [Params("таьadventures", "monday", "a")]
        public string Query { get; set; }

        public SearchBenchmarkV2()
        {
            _documentsSet = DocumentExtractor.DocumentsSet().Take(10000).ToArray();
            _index = new FullTextIndexV3();
            foreach (var item in _documentsSet)
                _index.AddStringToIndex(item);

        }

        [Benchmark(Baseline = true)]
        public void SimpleSearch()
        {
            new SimpleSearcherV2().SearchV3(Query, _documentsSet).ToArray();
        }

        [Benchmark]
        public void FullTextIndexSearch()
        {
            _index.SearchTest(Query).ToArray();
        }

    }
}
