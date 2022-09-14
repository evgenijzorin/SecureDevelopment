using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CertGenerator
{
    // содержит методы доступа к сертификатам пользователя, помещает их в специальную обертку X509Certificate2Wrapper.
    public class CertificateExplorerProvider
    {
        // имена хранилищь виндовс, для перебора сертификатов. Это те хранилища в которых мы будем искать сертификаты
        private string[] certStores = new string[] { "LocalMachine.My", "CurrentUser.My", "LocalMachine.Root", "CurrentUser.Root" };
                                            // "LocalMachine.My" - текущие сертификакты локального компьютера
                                            // "CurrentUser.My" - текущие сертификакты пользователя (личные)
                                            // "CurrentUser.My" - перед точкой наименование хранилища, после точки наименование локации. Т.е.
                                            // "CurrentUser" - "Сертификаты. Текущий пользователь"
                                            // "My" - интерпретировать как "Личные"

        // Поле для хранения группы сертификатов
        private List<X509Certificate2Wrapper> certList;
        // Наименования групп сертификатов для отображения в X509Certificate2Wrapper
        private const string CURRENT_USER_MY = "Текущий пользователь - Личные";
        private const string LOCAL_MACHINE_MY = "Локальный компьютер - Личные";
        private const string LOCAL_MACHINE_ROOT = "Локальный компьютер - Доверенные корневые центры сертификации";
        private const string CURRENT_USER_ROOT = "Текущий пользователь - Доверенные корневые центры сертификации";

        // запросить приватный ключ
        private bool requirePrivateKey;

        // Конструктор класса подразумевает запрос приватного ключа
        public CertificateExplorerProvider(bool requirePrivateKey)
        {
            this.requirePrivateKey = requirePrivateKey;
        }

        // Получить сертификаты
        public List<X509Certificate2Wrapper> Certificates
        {
            get { return certList; }
        }

        // Загрузить сертификаты из указанных в certStores хранилищ
        public void LoadCertificates()
        {
            certList = new List<X509Certificate2Wrapper>();
            foreach (string store in certStores)
            {
                certList.AddRange(LoadStore(store));
            }
        }

        /// <summary>
        /// Загрузить сертификаты из указанного хранилища
        /// </summary>
        /// <param name="storeName"> Специальное наименование хранилища (к примеру "CurrentUser.My")</param>
        /// <returns></returns>
        private List<X509Certificate2Wrapper> LoadStore(string storeName)
        {
            X509Store store = new X509Store(ExtractStoreName(storeName), ExtractStoreLocation(storeName));
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            try
            {
                // получить сертификаты X509Certificate2 обернутые в X509Certificate2Wrapper
                return CertificatesToView(store.Certificates, storeName);
            }
            finally
            {
                store.Close();
            }
                        // С помощью блока finally можно выполнить очистку всех ресурсов, выделенных в блоке try, и запускать код даже при возникновении
                        // исключения в блоке try. Как правило, операторы блока finally выполняются, когда элемент управления покидает оператор try.
                        // Передача управления может возникать в результате нормального выполнения, выполнения операторов break, continue, goto или return
                        // или распространения исключения из оператора try.
        }


        private List<X509Certificate2Wrapper> CertificatesToView(X509Certificate2Collection certificates,
            string groupName)
        {
            List<X509Certificate2Wrapper> certList = new List<X509Certificate2Wrapper>();
            foreach (X509Certificate2 cert in certificates)
            {
                string groupDesc = null;
                switch (groupName)
                {
                    case "CurrentUser.My":
                        groupDesc = CURRENT_USER_MY;
                        break;
                    case "LocalMachine.My":
                        groupDesc = LOCAL_MACHINE_MY;
                        break;
                    case "LocalMachine.Root":
                        groupDesc = LOCAL_MACHINE_ROOT;
                        break;
                    case "CurrentUser.Root":
                        groupDesc = CURRENT_USER_ROOT;
                        break;
                }
                if (requirePrivateKey)
                {
                    if (cert.HasPrivateKey)
                    {
                        certList.Add(new X509Certificate2Wrapper(cert, groupName, groupDesc));
                    }
                }
                else
                {
                    certList.Add(new X509Certificate2Wrapper(cert, groupName, groupDesc));
                }
            }
            return certList;
        }

        public static StoreName ExtractStoreName(string store)
        {
            return (StoreName)Enum.Parse(typeof(StoreName),
                store.Substring(store.IndexOf('.') + 1));
        }

        public static StoreLocation ExtractStoreLocation(string store)
        {
            return (StoreLocation)Enum.Parse(typeof(StoreLocation),
                store.Substring(0, store.IndexOf('.')));
        }

    }
}
