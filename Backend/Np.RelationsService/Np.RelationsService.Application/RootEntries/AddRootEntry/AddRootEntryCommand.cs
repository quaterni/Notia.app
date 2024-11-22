using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.RootEntries.AddRootEntry
{
    public sealed record AddRootEntryCommand(Guid NoteId) :ICommand;
}
