using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace EncryptMaster
{
    public class CacheProvider<T>
    {
        // массив рандомных значений для усложнения шифрования
        static byte[] _additionalEntropy = { 8, 9, 3, 4, 5, 1 };
        // зашифровать определенный список данных в файле
        public void CacheData (List<T> entitiesForEncrypt)
        {
            try
            {
                // 1. Сериализовать данные в Xml формат.
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));

                #region Debug save xlml to file
                /*
                using (FileStream fs = new FileStream("DataSerialised.xml", FileMode.OpenOrCreate))
                {
                    xmlSerializer.Serialize(fs, entitiesForEncrypt);
                    Console.WriteLine("Object has been serialized in to file DataSerialised.xml For debug");                    
                }*/
                #endregion

                // создать поток в памяти
                MemoryStream memoryStream = new MemoryStream();
                // Создать средство записывающее в поток памяти
                XmlWriter xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                // Связать сериализатор с xmlWriter, и сериализовать connections в память
                xmlSerializer.Serialize(xmlWriter, entitiesForEncrypt);

                // 2. Шифрукм данные сериализованные данные:
                byte[] protectedData = Protect(memoryStream.ToArray());
                // 3. Сохранить данные в файл (debug)
                File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory/*базовый каталог сборки*/} data.protected", protectedData);
            }

            catch (CryptographicException e)
            {
                Console.WriteLine($"CacheConnections error: {e.Message}");
            }
        }
        public List<T> GetDataFromCache() // List<T>
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
                // get byte array
                byte[] data = File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory} data.protected");
                // decipher
                data = Unprotect(data);
                // deserialize
                return (List<T>) xmlSerializer.Deserialize(new MemoryStream(data));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Deserialize data error: {e.Message}");
                return null;
            }
        }
        private byte[] Protect(byte[] data)
        {
            try
            {
                return ProtectedData.Protect(data,
                _additionalEntropy,
                DataProtectionScope.LocalMachine); // Доверенная область
            }
            catch (CryptographicException e) // Исключение при ошибке шифрования
            {
                Console.WriteLine($"Protect error {e.Message}");
                return null;
            }
        }
        private byte[] Unprotect(byte[] data)
        {
            try
            {
                return ProtectedData.Unprotect(data, _additionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e) // Исключение при ошибке шифрования
            {
                Console.WriteLine($"Unprotect error{e.Message}");
                return null;
            }
        }
    }
}