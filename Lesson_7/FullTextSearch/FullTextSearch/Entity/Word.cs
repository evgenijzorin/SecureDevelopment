using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextSearch.Entity
{
    [Table("Words")]
    [Index(nameof(Text), IsUnique = true)]
    public class Word
    { 

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Text { get; set; }
    }
}
