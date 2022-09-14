using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validstors
{
    public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
    {
        public CreateClientRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(1, 255);
            RuleFor(x => x.Surname)
                .NotEmpty()
                .Length(1, 255);
            RuleFor(x => x.Patronymic)
                .NotEmpty()    
                .Length(1, 255);
        }
    }
}
