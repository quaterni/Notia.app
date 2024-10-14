
using Np.RelationsService.Application.Abstractions.Messaging.Events;

namespace Np.RelationsService.Application.Notes.ApplicationEvents.NoteRemoved;

public sealed record  NoteRemovedApplicatonEvent(Guid NoteId) : IApplicationEvent;
