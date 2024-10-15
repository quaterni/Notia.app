using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Np.NotesService.Application.Relations.Service;

namespace Np.NotesService.Infrastructure.Messaging.Grpc;

internal class GrpcRelationsService : IRelationsService
{
    private readonly GrpcOptions _grpcOptions;

    public GrpcRelationsService(IOptionsSnapshot<GrpcOptions> options)
    {
        _grpcOptions = options.Get(GrpcOptions.Relations);
    }

    public async Task<IEnumerable<NoteResponse>> GetNotesFromRoot(CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_grpcOptions.Address);
        var client = new RelationsService.RelationsServiceClient(channel);

        var response = await client.GetNotesFromRootAsync(new GetNotesFromRootRequest(), cancellationToken: cancellationToken);

        var relationsResponse = response.Notes.Select(n=> new NoteResponse(new Guid(n.Id.Value)));
        return relationsResponse;
    }

    public async Task<IEnumerable<RelationResponse>> GetOutgoingRelations(Guid noteId, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_grpcOptions.Address);
        var client = new RelationsService.RelationsServiceClient(channel);

        var rawResponse = await client.GetOutgoingRelationsAsync(
            new GetOutgoingRelationsRequest()
            {
                Note = new GrpcNoteModel() { Id = new UUID() { Value = noteId.ToString() } }
            }, 
            cancellationToken: cancellationToken);

        var relations = new List<RelationResponse>();

        foreach (var rawRelation in rawResponse.Relations)
        {
            relations.Add(new RelationResponse(
                new Guid(rawRelation.Id.Value),
                new Guid(rawRelation.OutgoingNote.Id.Value),
                new Guid(rawRelation.IncomingNote.Id.Value)));
        }
        return relations;
    }

    public async Task<IEnumerable<RelationResponse>> GetIncomingRelations(Guid noteId, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_grpcOptions.Address);
        var client = new RelationsService.RelationsServiceClient(channel);

        var rawResponse = await client.GetIncomingRelationsAsync(
            new GetIncomingRelationsRequest() { Note = new GrpcNoteModel() { Id = new UUID() { Value = noteId.ToString() } } },
            cancellationToken: cancellationToken);

        var relations = new List<RelationResponse>();

        foreach (var rawRelation in rawResponse.Relations)
        {
            relations.Add(new RelationResponse(
                new Guid(rawRelation.Id.Value),
                new Guid(rawRelation.OutgoingNote.Id.Value),
                new Guid(rawRelation.IncomingNote.Id.Value)));
        }
        return relations;
    }
}