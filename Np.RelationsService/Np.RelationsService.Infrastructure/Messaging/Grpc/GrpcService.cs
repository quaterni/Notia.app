using Grpc.Core;
using MediatR;
using Np.RelationsService.Application.Relations.GetIncomingRelations;
using Np.RelationsService.Application.Relations.GetOutgoingRelations;
using Np.RelationsService.Application.RootEntries.GetRootEntries;
using RS = Np.RelationsService.Infrastructure.Messaging.Grpc.RelationsService;

namespace Np.RelationsService.Infrastructure.Messaging.Grpc;

public class GrpcService : RS.RelationsServiceBase
{
    internal readonly ISender _sender;

    public GrpcService(ISender sender)
   {
        _sender = sender;
    }

    public async override Task<GetNotesFromRootResponse> GetNotesFromRoot(GetNotesFromRootRequest request, ServerCallContext context)
    {
        var result = await _sender.Send(new GetRootEntriesQuery())!;

        if (result.IsFailed)
        {
            throw new ApplicationException("GrpcService result failed");
        }

        var response = new GetNotesFromRootResponse();

        foreach (var noteId in result.Value.NoteIds) 
        {
            response.Notes.Add(
                new GrpcNoteModel()
                {
                    Id = new UUID() { Value = noteId.ToString() }
                });
        }

        return response;
    }

    public async override Task<GetOutgoingRelationsResponse> GetOutgoingRelations(GetOutgoingRelationsRequest request, ServerCallContext context)
    {
        var noteId = new Guid(request.Note.Id.Value);

        var result = await _sender.Send(new GetOutgoingRelationsQuery(noteId));

        if (result.IsFailed)
        {
            throw new ApplicationException("GrpcService result failed");
        }

        var response = new GetOutgoingRelationsResponse();

        foreach(var relationItem in result.Value.Relations)
        {
            response.Relations.Add(new GrpcRelationModel()
            {
                Id = new UUID() { Value =  relationItem.Id.ToString() },
                IncomingNote = new GrpcNoteModel() { Id = new UUID() { Value = relationItem.IncomingId.ToString()} },
                OutgoingNote = new GrpcNoteModel() { Id = new UUID() { Value = relationItem.OutgoingId.ToString()} }
            });
        }

        return response;
    }

    public async override Task<GetIncomingRelationsResponse> GetIncomingRelations(GetIncomingRelationsRequest request, ServerCallContext context)
    {
        var noteId = new Guid(request.Note.Id.Value);

        var result = await _sender.Send(new GetIncomingRelationsQuery(noteId));

        if (result.IsFailed)
        {
            throw new ApplicationException("GrpcService result failed");
        }

        var response = new GetIncomingRelationsResponse();

        foreach (var relationItem in result.Value.Relations)
        {
            response.Relations.Add(new GrpcRelationModel()
            {
                Id = new UUID() { Value = relationItem.Id.ToString() },
                IncomingNote = new GrpcNoteModel() { Id = new UUID() { Value = relationItem.IncomingId.ToString() } },
                OutgoingNote = new GrpcNoteModel() { Id = new UUID() { Value = relationItem.OutgoingId.ToString() } }
            });
        }

        return response;
    }

}
