using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertGenerator
{
    internal class CertificateGenerationException : Exception
    {
        // Обертка над исключениями (чтобы исключения были типа специального для наших операций)
        public CertificateGenerationException(string message)
            : base (message)
        {

        }
    }
}
