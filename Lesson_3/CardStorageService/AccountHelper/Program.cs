using CardStorageService;

namespace AccountHelper
{
    class Programm
    {
        static void Main(string[] args)
        {
            var res = PasswordUtils.CreatePasswordHash("12345");
            Console.ReadKey();
        }
    }
}