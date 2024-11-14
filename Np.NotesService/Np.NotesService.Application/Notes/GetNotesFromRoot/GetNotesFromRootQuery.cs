using Np.NotesService.Application.Abstractions.Mediator;

namespace Np.NotesService.Application.Notes.GetNotesFromRoot;

public sealed record GetNotesFromRootQuery(string IdentityId) : UserRequest(IdentityId), IQuery<GetNotesFromRootResponse>;