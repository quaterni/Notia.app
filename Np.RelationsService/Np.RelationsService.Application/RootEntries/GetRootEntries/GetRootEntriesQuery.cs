
using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.RootEntries.GetRootEntries;

public sealed record GetRootEntriesQuery(Guid UserId) : IQuery<GetRootEntriesResponse>;
