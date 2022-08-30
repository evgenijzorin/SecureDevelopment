using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace jwtSample
{
    internal class UserService
    {
        // список пользователей и их паролей
        private IDictionary<string, string> _users = new Dictionary<string, string>()
        {
            {"root1", "test" },
            {"root2", "test" },
            {"root3", "test" },
            {"root4", "test" }
        };
        private const string SecretCode = "k333s6v9y/B?E(H+d"; // секретный код с колличеством символов не менее 16                                        
                                            
        public string Authenticate (string user, string password)
        {
            if (string.IsNullOrWhiteSpace(user)||
                    string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }
            int i = 0; // iterate counter, идентификатор пользователя
            foreach(KeyValuePair<string, string> pair in _users)
            {
                i++;
                if(string.CompareOrdinal(pair.Key, user)== 0 &&
                    string.CompareOrdinal(pair.Value, password)== 0) // Сравнивает два объекта String, оценивая числовые
                                                                     // значения соответствующих объектов Char в каждой строке.
                {
                    return GenerateJwtToken(i);
                }
            }
            return string.Empty;

        }

        private string GenerateJwtToken (int id)
        {
            // сгенерировать токен
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretCode); // сгенерировать 

            // класс хранящий описания для создания токена
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                // описание субъекта кодирования:
                Subject = new ClaimsIdentity(new Claim[]// Класс ClaimsIdentity представляет собой конкретную реализацию удостоверения на основе утверждений,
                                                        // то есть удостоверение, описанное коллекцией утверждений. 
                {
                    // составление коллекции кодирования:
                    new Claim(ClaimTypes.Name, // идентификатор кодирования 
                              id.ToString() // Закодированная сущьность                                        
                              )
                }),
                Expires = DateTime.UtcNow.AddMinutes(15), // время истечения токена// добавить к текущему времени 15 минут.
                // Получить подписанные реквизиты для входа
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)            
            };





            // создать токен
            SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token); // вернуть токен в качестве строки

        }

    }
}
