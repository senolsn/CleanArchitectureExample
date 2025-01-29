using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
