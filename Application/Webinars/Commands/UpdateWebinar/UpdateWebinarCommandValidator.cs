﻿using FluentValidation;

namespace Application.Webinars.Commands.UpdateWebinar
{
    public sealed class UpdateWebinarCommandValidator : AbstractValidator<UpdateWebinarCommand>
    {
        public UpdateWebinarCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.ScheduledOn).NotEmpty();
        }
    }
}
