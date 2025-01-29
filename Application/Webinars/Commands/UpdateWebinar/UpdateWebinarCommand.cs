using Application.Abstractions.Messaging;

namespace Application.Webinars.Commands.UpdateWebinar
{
    public sealed record UpdateWebinarCommand(Guid Id, string Name, DateTime ScheduledOn) : ICommand<Guid>;
}
