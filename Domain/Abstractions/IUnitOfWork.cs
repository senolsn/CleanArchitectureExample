using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IUnitOfWork //Concrete class büyük ihtimalle infrastructure içerisinde olacaktır.
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
