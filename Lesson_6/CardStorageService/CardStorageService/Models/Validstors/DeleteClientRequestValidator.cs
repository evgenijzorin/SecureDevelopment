using FluentValidation;
using CardStorageService.Models.Requests;

namespace CardStorageService.Models.Validstors
{
    public class DeleteClientRequestValidator : AbstractValidator<DeleteClientRequest>
    {
        public DeleteClientRequestValidator()
        {
            RuleFor(x => x.ClientId)
                .NotNull()
                .NotEmpty();
        }

    }
}
