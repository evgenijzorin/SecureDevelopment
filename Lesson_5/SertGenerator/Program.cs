using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("~~ Центр герерации сертификатов ~~~\n");
                Console.WriteLine("1. Создать корневой сертификат");
                Console.WriteLine("2. Создать сертификат");
                Console.WriteLine("Выберите подпрограмму (0 - завершение работы приложения): ");
                if (int.TryParse(Console.ReadLine(), out int no)) // преобразовать данные и ввернуть булево значение
                {
                    switch (no)
                    {
                        case 0:
                            Console.WriteLine("Завершение работы приложения");
                            Console.ReadKey();
                            return;
                        case 1: //  Создать корневой сертификат
                            CertificateConfiguration certificateConfiguration = new CertificateConfiguration
                            {
                                CertName = "Рога и копыта CA", // CA - center authority
                                OutFolder = @"D:\cert", // папка для сертификата
                                Password = "12345678",
                                CertDuration = 30 // 30 лет
                            };
                            CertificateGenerationProvider certificateGenerationProvider = new CertificateGenerationProvider();
                            certificateGenerationProvider.GenerateRootSertificate(certificateConfiguration);

                            break;
                        case 2:
                            int counter = 0; // счетчик найденных сертификатов
                            CertificateExplorerProvider certificateExplorerProvider = new CertificateExplorerProvider(true);
                            // Найти сертификаты с закрытым ключом
                            certificateExplorerProvider.LoadCertificates();
                            // Вывести сертификаты на экран 
                            Console.WriteLine("Корневые сертификаты:");
                            foreach (var certificate in certificateExplorerProvider.Certificates)
                            {
                                Console.WriteLine($"{counter++} >>> {certificate}");
                            }
                            Console.WriteLine("Укажите номер корневого сертификата:");
                            // Свойства будующего сертификата:
                            CertificateConfiguration addCertificateConfiguration = new CertificateConfiguration
                            {
                                RootCertificate = certificateExplorerProvider.Certificates[int.Parse(Console.ReadLine())].Certificate,
                                CertName = "IT Департамент",
                                OutFolder = @"D:\cert",
                                Password = "12345678",
                            };
                            // Создание сертификата
                            CertificateGenerationProvider certificateGenerationProvider2 = new CertificateGenerationProvider();
                            certificateGenerationProvider2.GenerateCertificate(addCertificateConfiguration);
                            Console.WriteLine("Сертификат сгенерирован!");
                            break;
                        default:
                            Console.WriteLine("Некорректный номер подпрограммы. Пожалуйста повторите ввод");
                            break;
                    }
                }
                else
                    Console.WriteLine("Некорректный номер подпрограммы. Пожалуйста повторите ввод");
            }
        }
    }
}
