using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Primitives
{
    public abstract class Entity
    {
        //Protected Constructor : Sadece kendisini miras alan sınıflar tarafından erişilebilir.
        protected Entity(Guid id) => Id = id;

        protected Entity()
        {

        }

        public Guid Id { get; set; }
    }
}
