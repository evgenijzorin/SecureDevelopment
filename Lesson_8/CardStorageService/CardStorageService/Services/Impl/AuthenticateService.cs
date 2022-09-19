using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CardStorageService.Services.Impl
{
    public class AuthenticateService : IAuthenticateService
    {
        #region Services
        // для того чтобы взаимодействовать между сервисами с разным верменем жизни (singleton и scouped) используем serviceScopeFactory.
        // Этот сервис позволяет воспользоваться scouped сервисами
        private readonly IServiceScopeFactory _serviceScopeFactory;
        #endregion
        // коллекция открытых сессий (работающих токенов)
        private readonly Dictionary<string, SessionInfo> _sessions = new Dictionary<string, SessionInfo>();

        #region Constructor
        public AuthenticateService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        #endregion


        // Уникальный ключ для создания токенов 
        public const string SecretKey = "HHHdre_33223dasf)+8";

        /// <summary> Найти сессию (проверить на существование) </summary>
        public SessionInfo GetSessionInfo(string sessionToken)
        {
            SessionInfo sessionInfo;
            lock(_sessions)
            {
                _sessions.TryGetValue(sessionToken, out sessionInfo);
                if (sessionInfo == null)
                {
                    // Получить контекст базы данных из scope lifeTime
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    CardStorageServiceDbContext context = scope.ServiceProvider.GetService<CardStorageServiceDbContext>();

                    // Find session with equal tokens
                    AccountSession session = context.AccountSessions
                        .FirstOrDefault(item => item.SessionToken == sessionToken);                                        
                    if (session == null)
                        return null;
                    Account account = context.Accounts.FirstOrDefault(item => item.AccountId == session.AccountId);
                    // Получить информацию о сесси
                    sessionInfo = NewSessionInfo(account, session);
                    if (sessionInfo != null)
                    {
                        // Добавить сессию в коллекцию
                        _sessions[sessionToken] = sessionInfo;
                    }    

                }
                return sessionInfo;
            }
        }

        public AuthenticationResponse Login(AuthenticationRequest authenticationRequest)
        {
            // получить scope сервисы, которые содержат зависимости с временем жизни scoped
            using IServiceScope scope = _serviceScopeFactory.CreateScope(); // объявление using без скобок. (это тоже для уничтожения dispose)

            CardStorageServiceDbContext context = scope.ServiceProvider. // проводник сервисов
                GetRequiredService<CardStorageServiceDbContext>(); // получить необходимый сервис контекст базы данных

            // Проверка существования пользователя
            Account account =
                !string.IsNullOrWhiteSpace(authenticationRequest.Login)
                ?
                FindeAccountByLogin(context, authenticationRequest.Login) : null;
            if (account == null)
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.UserNotfound
                };
            }

            // Верификация пользователя
            if (!PasswordUtils.VerifyPassword(authenticationRequest.Password, account.PasswordSalt, account.PasswordHash))
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.IvalidPassword
                };
            }

            // Создаем новую сессию
            AccountSession session = new AccountSession
            {
                AccountId = account.AccountId,
                SessionToken = GenerateSessionToken(account),
                TimeCreated = DateTime.Now,
                TimeLastRequest = DateTime.Now,
                IsClosed = false,

            };
            // Добавить новую сессию в таблицу сессий _sessions
            context.AccountSessions.Add(session);
            context.SaveChanges();

            // Добавить информацию о сессии в список 
            SessionInfo sessionInfo = NewSessionInfo(account, session);
              
            // Добавить сессию в список сессий используя блокировку объекта для потокобезопасности. 
            // Алтернативно для этого можно использовать потокобезопасную коллекцию.
            lock(_sessions)
            {
                _sessions.Add(session.SessionToken, sessionInfo);
            }            

            return new AuthenticationResponse
            {
                SessionInfo = sessionInfo,
                Status = AuthenticationStatus.Success                
            };
        }

        private SessionInfo NewSessionInfo (Account account, AccountSession accountSession)
        {
            return new SessionInfo
            {
                SessionId = accountSession.SessionId,
                SessionToken = accountSession.SessionToken,
                AccountDto = new AccountDto
                {
                    AccountId = account.AccountId,
                    EMail = account.EMail,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    Locked = account.Locked,
                    SecondName = account.SecondName
                }
            };
        }

        private Account FindeAccountByLogin(CardStorageServiceDbContext context, string login)
        {
            return context
                .Accounts
                .FirstOrDefault(account => account.EMail == login);
        }

        private string GenerateSessionToken(Account account)
        {
            // сгенерировать токен
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey); // сгенерировать 

            // класс хранящий описания для создания токена
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                // описание субъекта кодирования:
                Subject = new ClaimsIdentity(new Claim[]// Класс ClaimsIdentity представляет собой конкретную реализацию удостоверения на основе утверждений,
                                                        // то есть удостоверение, описанное коллекцией утверждений. 
                {
                    // составление коллекции кодирования:
                    new Claim(ClaimTypes.NameIdentifier, // идентификатор кодирования 
                              account.AccountId.ToString() // Закодированная сущьность                                        
                              ),
                    new Claim(ClaimTypes.Name, account.EMail)
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
