using System;
using System.Security.Cryptography;
using System.Text;

namespace CardStorageService
{
    public class PasswordUtils
    {
        private const string SecretKey = "ewewuoirHFD-s123==";
        /// <summary>
        /// Метод создания комплексного Salt-Hash-Key ключа 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static (string passwordSalt, string passwordHash) CreatePasswordHash(string password)
        {
            // generate random salt
            byte[] buffer = new byte[16];
            RNGCryptoServiceProvider secureRandom = new RNGCryptoServiceProvider(); //Реализует криптографический генератор случайных чисел,
                                                                                    //используя реализацию, предоставляемую поставщиком служб шифрования (CSP).
            secureRandom.GetBytes(buffer); // заполнить байтовый массив рандомными значениями  

            // create hash
            string passwordSalt = Convert.ToBase64String(buffer); // кодировать значение в эквивалентную стороку согласно кодированию Base64 
            string passwordHash = GetPasswordHash(password, passwordSalt);

            // done
            return (passwordSalt, passwordHash);
        }
        /// <summary> получить строку пароля соединив pass, salt, Key и вычислив из этого хэш </summary>
        public static string GetPasswordHash(string password, string passwordSalt)
        {
            // build password string
            password = $"{password}~{passwordSalt}~{SecretKey}";
            byte[] buffer = Encoding.UTF8.GetBytes(password); // получить последовательность байт согласно кодированию юникода

            SHA512 sha512 = new SHA512Managed();
            byte[] passwordHash = sha512.ComputeHash(buffer); // Вычислить хэшь по алгоритму SHA512

            // done/ переводим последовательность байт в строку стороку согласно кодированию Base64 
            return Convert.ToBase64String(passwordHash);           
        }


        /// <summary>
        /// Верификация пароля
        /// </summary>
        /// <param name="password"> Пароль введенный пользователем </param>
        /// <param name="passwordSalt"> Соль, хранящаяся в базе данных </param>
        /// <param name="passwordHash"> хэшированный пароль, хранящаяся в базе данных</param>
        /// <returns></returns>
        public static bool VerifyPassword(string password, string passwordSalt, string passwordHash)
        {
            return GetPasswordHash(password, passwordSalt) == passwordHash;
        }
                /* Используется три компонента :
         1. Пароль пользователя, (вводится пользователем и нигде не хранится) 
         2. Соль - хэш усложняющий пароль, хранится в базе данных. Вычисляется один раз при регистрации пароля.
         3. Секретный ключь, недоступный из базы данных. 
            Получается в базе данных хранится соль и passwordHash. При верификации вводится пароль, из базы данных берется passwordHash   
            вычисляется passwordHash из соли и пароля пользователя. Затем сравниваются 2 passwordHash */
    }
}
