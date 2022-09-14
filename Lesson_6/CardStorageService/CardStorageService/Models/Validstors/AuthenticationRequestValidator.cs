using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validstors
{
    public class AuthenticationRequestValidator : AbstractValidator
        <AuthenticationRequest> // Тип который валидируем 
    {
        public AuthenticationRequestValidator()
        {
            // Определяем правила валидации для свойств AuthenticationRequest
            RuleFor(x => x.Login)                
                .NotNull()
                .Length(5, 255)
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotNull()
                .Length(5, 50);
        }                                                                    
    }
}
