using FluentValidation;

namespace Application.Webinars.Commands.DeleteWebinar
{
    public sealed class DeleteWebinarCommandValidator : AbstractValidator<DeleteWebinarCommand>
    {
        public DeleteWebinarCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
