using Domain.Entities;

namespace Domain.Abstractions
{
    public interface IWebinarRepository
    {
        void Insert(Webinar webinar);
        void Update(Webinar webinar);
        Task<Webinar> GetWebinarById(Guid id);
    }
}
