using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Domain.Abstractions
{
    public abstract class Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        protected Entity()
        {
        }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public Guid Id { get;}

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear(); 
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity entity && entity.Id.Equals(Id) ;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
