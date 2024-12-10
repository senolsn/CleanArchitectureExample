using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Webinars.Commands.CreateWebinar
{
    public sealed class CreateWebinarCommandValidator : AbstractValidator<CreateWebinarCommand>
    {
        public CreateWebinarCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.ScheduledOn).NotEmpty();
        }
    }
}
