using Application.Abstractions.Messaging;
using Domain.Abstractions;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Webinars.Commands.DeleteWebinar
{
    internal sealed class DeleteWebinarCommandHandler : ICommandHandler<DeleteWebinarCommand,Guid>
    {
        private readonly IWebinarRepository _webinarRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWebinarCommandHandler(IUnitOfWork unitOfWork, IWebinarRepository webinarRepository)
        {
            _unitOfWork = unitOfWork;
            _webinarRepository = webinarRepository;
        }

        public async Task<Guid> Handle(DeleteWebinarCommand request, CancellationToken cancellationToken)
        {
            var webinar = await _webinarRepository.GetWebinarById(request.Id);

            if (webinar is null)
            {
                throw new WebinarNotFoundException(request.Id);
            }

            _webinarRepository.Delete(webinar);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
             
            return webinar.Id; //Webinar bellekte durduğu için webinar.Id null dönmez.
        }
    }
}
