namespace Application.Webinars.Commands.UpdateWebinar
{
    public sealed record UpdateWebinarCommandRequest(Guid Id, string Name, DateTime ScheduledOn);
}
