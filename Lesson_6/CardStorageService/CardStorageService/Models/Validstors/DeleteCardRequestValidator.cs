using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validstors
{
    public class DeleteCardRequestValidator : AbstractValidator<DeleteCardRequest>
    {
        public DeleteCardRequestValidator()
        {
            RuleFor(x => x.CardId)
                .NotEmpty()
                .NotNull();                
        }

    }
}
