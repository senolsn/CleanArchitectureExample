using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Exceptions;

namespace Application.Webinars.Commands.UpdateWebinar
{
    internal sealed class UpdateWebinarCommandHandler : ICommandHandler<UpdateWebinarCommand,Guid>
    {
        private readonly IWebinarRepository _webinarRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWebinarCommandHandler(IUnitOfWork unitOfWork, IWebinarRepository webinarRepository)
        {
            _unitOfWork = unitOfWork;
            _webinarRepository = webinarRepository;
        }

        public async Task<Guid> Handle(UpdateWebinarCommand request, CancellationToken cancellationToken)
        {
            var webinar = await _webinarRepository.GetWebinarById(request.Id);

            if (webinar is null)
            {
                throw new WebinarNotFoundException(request.Id);
            }

            webinar.Name = request.Name;
            webinar.ScheduledOn = request.ScheduledOn;

            _webinarRepository.Update(webinar);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return webinar.Id;
        }
    }
}
