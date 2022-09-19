using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch.Services.impl
{
    public class DocumentExtractor : IDocumentExtractor
    {
        // Возвращает массив строк - отдельные статьи без номера
        public static IEnumerable<string> DocumentsSet()
        {
            return ReadDocuments(AppContext.BaseDirectory + "sample\\" + "data.txt");
        }

        private static IEnumerable<string> ReadDocuments(string fileName)
        {
            using (var streamReader = new StreamReader(fileName))
            {
                while(!streamReader.EndOfStream)
                {
                    var doc=streamReader.ReadLine()?.Split('\t'); // ?. - Используется доступ к членам с проверкой на null. 
                                                                  // Тоесть если streamReader.ReadLine() возвращает null то вернуть null. если отлично от null то
                                                                  // выполнить: .Split('\t')
                    yield return doc[1]; // Использование слова yield означает, что метод, оператор или метод доступа get, в котором присутствует это ключевое слово,
                                         // является итератором. (тоесть можно перебрать foreach
                }
            }
        }
    }
}
