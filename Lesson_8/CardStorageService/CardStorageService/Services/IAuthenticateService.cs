using CardStorageService.Models;
using CardStorageService.Models.Requests;

namespace CardStorageService.Services
{
    public interface IAuthenticateService
    {
        AuthenticationResponse Login(AuthenticationRequest authenticationRequest);
        // Проверка валидности токена
        public SessionInfo GetSessionInfo(string sessionToken); 
    }
}
