using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Np.NotesService.Application.Relations.Service;
using System.Runtime.CompilerServices;
using static Np.NotesService.Infrastructure.Messaging.Grpc.RelationsService;

namespace Np.NotesService.Infrastructure.Messaging.Grpc;

internal partial class GrpcRelationsService : IRelationsService
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Connecting to relations service: {Address}")]
    static partial void LogConnectingToService(ILogger logger, string address);

    [LoggerMessage(Level = LogLevel.Information, Message = "Connection to relations service created: {Address}")]
    static partial void LogConnectionCreated(ILogger logger, string address);


    [LoggerMessage(Level = LogLevel.Error, Message = "Procedure call failed: {ProcedureName}\nException: {ExceptionMessage}")]
    static partial void LogCallFailed(ILogger logger, string procedureName, string exceptionMessage);

    [LoggerMessage(Level = LogLevel.Information, Message = "Procedure call was success: {ProcedureName}")]
    static partial void LogCallSeccessed(ILogger logger, string procedureName);

    private readonly GrpcOptions _grpcOptions;
    private readonly ILogger<GrpcRelationsService> _logger;

    public GrpcRelationsService(
        IOptionsSnapshot<GrpcOptions> options,
        ILogger<GrpcRelationsService> logger)
    {
        _grpcOptions = options.Get(GrpcOptions.Relations);
        _logger = logger;
    }

    private async Task<TResponse> CallClient<TResponse>(
        Func<RelationsServiceClient,Task<TResponse>> clientCallback, 
        [CallerMemberName] string memberName="")
    {
        try
        {
            LogConnectingToService(_logger, _grpcOptions.Address);
            using var channel = GrpcChannel.ForAddress(_grpcOptions.Address);
            LogConnectionCreated(_logger, _grpcOptions.Address);

            var client = new RelationsServiceClient(channel);
            var response = await clientCallback(client);

            LogCallSeccessed(_logger, memberName);
            return response;

        }
        catch (Exception ex) 
        {
            LogCallFailed(_logger, memberName, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<NoteResponse>> GetNotesFromRoot(CancellationToken cancellationToken)
    {
        return await CallClient(async client =>
        {
            var response = await client.GetNotesFromRootAsync(new GetNotesFromRootRequest(), cancellationToken: cancellationToken);

            var relationsResponse = response.Notes.Select(n => new NoteResponse(new Guid(n.Id.Value)));
            return relationsResponse;
        });
    }

    public async Task<IEnumerable<RelationResponse>> GetOutgoingRelations(Guid noteId, CancellationToken cancellationToken)
    {
        return await CallClient(async client =>
        {
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
        });
    }

    public async Task<IEnumerable<RelationResponse>> GetIncomingRelations(Guid noteId, CancellationToken cancellationToken)
    {
        return await CallClient(async client =>
        {
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
        });
    }
}