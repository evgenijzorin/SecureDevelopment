using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace CertGenerator
{
    // класс хранит данные для построения сертификата
    public class CertificateConfiguration
    {
        public X509Certificate2 RootCertificate { get; set; } // корневой сертификат
            // Структура X.509 возникла в рабочей группе Международной организации по стандартизации(ISO).
            // Эту структуру можно использовать для представления различных типов информации, включая удостоверения, права и атрибуты владельца(разрешения,
            // возраст, пол, расположение, принадлежность и т.д.). Несмотря на то, что спецификации ISO наиболее информативны по самой структуре,
            // X509Certificate2 класс предназначен для моделирования сценариев использования, определенных в спецификациях, выданных инфраструктурой
            // открытых ключей(IETF),
        public int CertDuration { get; set; } // продолжительность жизни сертификата
        public string CertName { get; set; } // имя сертификата
        public string Password { get; set; }  // пароль к закрытому ключу
        public string OutFolder { get; set; } // папка куда выгружаются готовые сертификаты
        public string Email { get; set; } // если использовать сертификат при валидации почты
    }
}
