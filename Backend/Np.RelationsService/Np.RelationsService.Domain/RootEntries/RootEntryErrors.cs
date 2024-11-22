using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Domain.RootEntries
{
    public class RootEntryErrors
    {
        public static Error Duplication => new("[RootEntry]: Attempting add root entry twice");

        public static Error NotFound => new("[RootEntry]: Root entry not found");
    }
}
