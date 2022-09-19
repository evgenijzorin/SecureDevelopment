using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validstors
{
    public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        public CreateCardRequestValidator()
        {
            RuleFor(x => x.CardNo)
                .NotEmpty()
                .NotNull()                
                .CreditCard();
            RuleFor(x => x.CVV2)
                .NotEmpty()
                .NotNull()                
                .Length(3);
            RuleFor(x => x.Name)
                .NotNull()
                .Length(1, 50);
            RuleFor(x => x.ClientId)
                .NotNull(); 
        }
    }
}
