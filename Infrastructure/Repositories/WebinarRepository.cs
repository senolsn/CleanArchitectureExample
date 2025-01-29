using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class WebinarRepository : IWebinarRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WebinarRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public void Insert(Webinar webinar) => _dbContext.Set<Webinar>().Add(webinar);
        public void Update(Webinar webinar) => _dbContext.Set<Webinar>().Update(webinar);
        public void Delete(Webinar webinar) => _dbContext.Set<Webinar>().Remove(webinar);
        public async Task<Webinar> GetWebinarById(Guid id) => await _dbContext.Set<Webinar>().FirstAsync(x => x.Id == id);
    }
}
