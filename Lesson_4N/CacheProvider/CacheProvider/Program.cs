
using EncryptMaster;
namespace CacheProviderTest;
class Programm
{
    static void Main(string[] args)
    {
        CacheProvider<Car> cacheProvider = new CacheProvider<Car>();
        // Проверка существования записанных данных и их запись или извлечение
        if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory} data.protected"))
        {
            try
            {
                // Расшифрованне данные:
                List<Car> carsDecipher = cacheProvider.GetDataFromCache();
                Console.WriteLine("File decrypted:");
                foreach (Car car in carsDecipher)
                {
                    
                    Console.WriteLine($"id: {car.Id}, Модель: {car.NameModel}, Владелец: {car.Owner}, цена: {car.Price}, дата выпуска: {car.ProductionDate.ToString()}");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("decryption error");
            }
        }
        else
        {
            List<Car> cars = new List<Car>
            {
            new Car
            {
                Id = 1,
                NameModel = "Toyota prado",
                Owner = "Иванов Иван Иванович",
                Price = 100_000M,
                ProductionDate = new DateTime(1977, 1, 2).ToUniversalTime()
            },
            new Car
            {
                Id = 2,
                NameModel = "ваз 2106",
                Owner = "Сергеев Сергей Сергеевич",
                Price = 200_000M,
                ProductionDate = new DateTime(1990, 12, 2).ToUniversalTime()
            }
        };
            try
            {
                cacheProvider.CacheData(cars);
                Console.WriteLine("Object has been encrepted in to file  data.protected");
            }
            catch(Exception e)
            {
                Console.WriteLine("encryption error");
            }
        }
    }
}






