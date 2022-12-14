using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace CardStorageService.Controllers
{
    [Authorize]
    [Route("api/auth")] // атрибут указывает на маршрут контроллера
    [ApiController]
    public class AuthenticateController : Controller
    {
        #region Services
        private readonly IAuthenticateService _authenticateService;
        #endregion

        #region Constructors
        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]        
        [ProducesResponseType(typeof(AuthenticationResponse),  // фильтр возвращаемых значений // typeof - возвратит тип сущности
            StatusCodes. // коллекция кодов состояния HTTP.
            Status200OK)] // Код состояния HTTP 200. - успешно
        public IActionResult Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            AuthenticationResponse authenticationResponse = _authenticateService.Login(authenticationRequest);
            // checking authentication status 
            if (authenticationResponse.Status == Models.AuthenticationStatus.Success)
            {                
                // добавить заголовок ответа в коллекцию заголовков
                Response // свойство объекта ControllerBase HttpResponse Возвращает значение для выполняемого действия.
                    .Headers // Возвращает заголовки ответа IHeaderDictionary.
                        .Add( // добавить заголовок
                            "X-Session-Token", // стандартный ключь заголовка в рамках которого передаются токены (не нашел докум.)
                            authenticationResponse.SessionInfo.SessionToken); // Токен
            }
            return Ok(authenticationResponse);
        }

        [HttpGet]
        [Route("session")]
        [ProducesResponseType(typeof(SessionInfo), StatusCodes.Status200OK)]
        public IActionResult GetSessionInfo()
        {
            Microsoft.Extensions.Primitives.StringValues authorization 
                = Request // получить Http запрос для исполняемого действия
                .Headers[HeaderNames.Authorization]; // Найти авторизацию в коллекции заголовков
            // По правилам токен прередается в следующем синтаксисе:
            // Bearer xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            // убрать кодовое слово "Bearer " из предоставленного токена и вернуть сам токен
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                // схема авторизации - ключевое слово "Bearer "
                var scheme = headerValue.Scheme;
                // Сам токен
                var sessionToken = headerValue.Parameter;
                if (string.IsNullOrEmpty(sessionToken))
                    return Unauthorized(); // Код состояния HTTP 401. - неавторизован. Стандартная ошибка
                // Проверка существования сессии
                SessionInfo sessionInfo = _authenticateService.GetSessionInfo(sessionToken);
                if (sessionInfo == null)
                    return NotFound(); // Код состояния HTTP 404. - ненайден.
                return Ok(sessionInfo);
            }
            return Unauthorized();
        }
    }
}

