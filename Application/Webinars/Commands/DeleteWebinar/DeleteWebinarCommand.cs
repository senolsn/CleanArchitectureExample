using Application.Abstractions.Messaging;

namespace Application.Webinars.Commands.DeleteWebinar
{
    public sealed record DeleteWebinarCommand(Guid Id) : ICommand<Guid>;
}
