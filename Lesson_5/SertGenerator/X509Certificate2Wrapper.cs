using System.Security.Cryptography.X509Certificates;

namespace CertGenerator
{
    // Класс обертка (анг. Wrapper) над объектом X509Certificate  для оформленного отображения 
    public class X509Certificate2Wrapper
    {
        private X509Certificate2 cert = null;
        private string group = null;
        private string certGroupName = null;

        public X509Certificate2Wrapper(X509Certificate2 cert, string certGroupName, string group)
        {
            this.cert = cert;
            this.certGroupName = certGroupName;
            this.group = group;
        }

        // получить сам X509Certificate2
        public X509Certificate2 Certificate
        {
            get { return cert; }
        }

        // опубликован для
        public string PublishedFor
        {
            get { return cert.GetNameInfo(X509NameType.SimpleName, false); }
            // Значение true для включения имени поставщика; для включения имени получателя — false.
        }

        // опубликован (кем)
        public string Published
        {
            get { return cert.GetNameInfo(X509NameType.SimpleName, true); }
        }

        // получить дату истечения
        public string ExpirationDate
        {
            get { return cert.GetExpirationDateString(); }
        }

        // группа сертификатов (для того чтобы разделять сертификаты на группы по какому либо признаку)
        public string Group
        {
            get { return group; }
        }

        public string CertGroupName
        {
            get { return certGroupName; }
        }

        // Переопределен метод для отображения в тексте, в читаемом виде.
        public override string ToString()
        {
            return $"Group: {Group} ({CertGroupName})\nPublishedFor: {PublishedFor}\nPublished: {Published}\nExp: {ExpirationDate}\n";
        }
    }
}
