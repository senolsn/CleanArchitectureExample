namespace Application.Webinars.Commands.UpdateWebinar
{
    public sealed record UpdateWebinarCommandRequest(string Name, DateTime ScheduledOn);
}
