using Domain.Primitives;


namespace Domain.Entities
{
    public sealed class Webinar : Entity
    {
        public Webinar(Guid id, string name, DateTime scheduledOn)
            :base(id)
        {
            Name = name;
            ScheduledOn = scheduledOn;
        }

        private Webinar()
        {
            
        }

        public string Name { get; set; }
        public DateTime ScheduledOn { get; set; }
    }
}
