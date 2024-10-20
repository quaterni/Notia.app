using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Domain.Abstractions
{
    public abstract class Entity
    {
        private List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

        protected Entity()
        {
        }

        protected Entity(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public override bool Equals(object? obj)
        {
            return obj is Entity e && e.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
