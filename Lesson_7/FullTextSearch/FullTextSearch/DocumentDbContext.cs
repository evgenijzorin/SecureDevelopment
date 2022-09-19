using FullTextSearch.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch
{
    public class DocumentDbContext : DbContext
    {
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<WordDocument> WordDocuments { get; set; }

        // конструктор
        public DocumentDbContext(DbContextOptions option) : base(option) { }
    }
}
