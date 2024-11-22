using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.ApplicationEvents.Notes.NoteRemoved;

public sealed record NoteRemovedApplicatonEvent(Guid NoteId) : IApplicationEvent;
