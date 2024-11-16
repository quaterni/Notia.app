using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Np.NotesService.Application.Dtos;
using Np.NotesService.Application.Relations;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Infrastructure.Messaging.Grpc.Relations;
using System.Runtime.CompilerServices;
using static Np.NotesService.Infrastructure.Messaging.Grpc.Relations.RelationsService;


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
    private readonly INotesRepository _notesRepository;

    public GrpcRelationsService(
        IOptionsSnapshot<GrpcOptions> options,
        ILogger<GrpcRelationsService> logger,
        INotesRepository notesRepository)
    {
        _grpcOptions = options.Get(GrpcOptions.Relations);
        _logger = logger;
        _notesRepository = notesRepository;
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

    public async Task<IEnumerable<NoteItemDto>> GetNotesFromRoot(Guid userId, CancellationToken cancellationToken)
    {
        return await CallClient(async client =>
        {
            var response = await client.GetNotesFromRootAsync(
                new GetNotesFromRootRequest() 
                { 
                    UserId = new Relations.GrpcUuid() { Value = userId.ToString() }
                }, 
                cancellationToken: cancellationToken);

            var noteIds = response.NoteIds.Select(grpcNoteId => Guid.Parse(grpcNoteId.Value));
            var notes = new List<NoteItemDto>();
            foreach(var noteId in noteIds)
            {
                var note = await _notesRepository.GetNoteById(noteId) ?? throw new ApplicationException("Note id from relations service not found");
                notes.Add(new NoteItemDto(note.Title, note.Id));
            }

            return notes;
        });
    }

    public async Task<IEnumerable<NoteItemDto>> GetOutgoingNotes(Guid noteId, CancellationToken cancellationToken)
    {
        return await CallClient(async client =>
        {
            var rawResponse = await client.GetOutgoingRelationsAsync(
            new GetOutgoingRelationsRequest()
            {
                NoteId = new Relations.GrpcUuid() { Value =noteId.ToString() }
            },
            cancellationToken: cancellationToken);

            var notes = new List<NoteItemDto>();

            foreach (var rawNoteId in rawResponse.NoteIds)
            {
                var noteId = Guid.Parse(rawNoteId.Value);
                var note = await _notesRepository.GetNoteById(noteId) 
                    ?? throw new ApplicationException("Note id from relations service not found"); 
                notes.Add(new NoteItemDto(note.Title, note.Id));
            }
            return notes;
        });
    }

    public async Task<IEnumerable<NoteItemDto>> GetIncomingNotes(Guid noteId, CancellationToken cancellationToken)
    {
        return await CallClient(async client =>
        {
            var rawResponse = await client.GetIncomingRelationsAsync(
                new GetIncomingRelationsRequest() { NoteId = new Relations.GrpcUuid { Value = noteId.ToString() } },
                cancellationToken: cancellationToken);

            var notes = new List<NoteItemDto>();

            foreach (var rawNoteId in rawResponse.NoteIds)
            {
                var noteId = Guid.Parse(rawNoteId.Value);
                var note = await _notesRepository.GetNoteById(noteId) 
                    ?? throw new ApplicationException("Note id from relations service not found");
                notes.Add(new NoteItemDto(note.Title, note.Id));
            }
            return notes;
        });
    }
}