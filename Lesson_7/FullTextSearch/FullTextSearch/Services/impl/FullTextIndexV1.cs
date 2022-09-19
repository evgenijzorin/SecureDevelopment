using FullTextSearch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch.Services.impl
{
    internal class FullTextIndexV1
    {
        private readonly DocumentDbContext _context;
        private readonly Lexer _lexer = new Lexer();

        public FullTextIndexV1(DocumentDbContext context = null)
        {
            _context = context;
        }

        public void BuildIndex()
        {
            // Перебор статей в документе.
            foreach (var document in _context.Documents.ToArray())
            {
                // Получить слова в статье
                var wordsInArticle = _lexer.GetTokens(document.Content);
                // Перебор слов в статье
                foreach (var token in wordsInArticle)
                {
                    // поиск слова в базе данных слов
                    var word = _context.Words.FirstOrDefault(w => w.Text == token);
                    int wordId = 0;
                    // если слово не было найдено добавляем его
                    if (word == null)
                    {
                        var wordObj = new Word
                        {
                            Text = token
                        };
                        _context.Words.Add(wordObj);
                        _context.SaveChanges();
                        wordId = wordObj.Id;
                    }
                    else
                        // если слово было найдено присваеваем ему идентификатаор из базы
                        wordId = word.Id;

                    // присваиваем в таблице слово-в-документе
                    var wordDocument = _context.WordDocuments.FirstOrDefault(wd => wd.WordId == wordId && wd.DocumentId == document.Id);
                    if (wordDocument == null)
                    {
                        _context.WordDocuments.Add(new WordDocument
                        {
                            DocumentId = document.Id,
                            WordId = wordId
                        });
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}
