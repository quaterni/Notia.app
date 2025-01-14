﻿
using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Domain.Notes.Events;

public sealed record class NoteRemovedEvent(Guid NoteId) : IDomainEvent;
