using Grpc.Core;
using MediatR;
using Np.RelationsService.Application.Relations.GetIncomingRelations;
using Np.RelationsService.Application.Relations.GetOutgoingRelations;
using Np.RelationsService.Application.RootEntries.GetRootEntries;
using Np.RelationsService.Infrastructure.Migrations;
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
        if (!Guid.TryParse(request.UserId.Value, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is not valid"));
        }

        var result = await _sender.Send(new GetRootEntriesQuery(userId))!;
        if (result.IsFailed)
        {
            throw new ApplicationException("GrpcService result failed");
        }

        var response = new GetNotesFromRootResponse();
        foreach (var noteId in result.Value.NoteIds) 
        {
            response.NoteIds.Add(
                new GrpcUuid()
                {
                    Value = noteId.ToString()
                });
        }
        return response;
    }

    public async override Task<GetOutgoingRelationsResponse> GetOutgoingRelations(GetOutgoingRelationsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.NoteId.Value, out var noteId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is not valid"));
        }
        var result = await _sender.Send(new GetOutgoingRelationsQuery(noteId));

        if (result.IsFailed)
        {
            throw new ApplicationException("GrpcService result failed");
        }

        var response = new GetOutgoingRelationsResponse();
        foreach(var outgoingNoteId in result.Value.OutgoingNoteIds)
        {
            response.NoteIds.Add(new GrpcUuid()
            {
                Value = outgoingNoteId.ToString()
            });
        }
        return response;
    }

    public async override Task<GetIncomingRelationsResponse> GetIncomingRelations(GetIncomingRelationsRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.NoteId.Value, out var noteId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "UserId is not valid"));
        }
        var result = await _sender.Send(new GetIncomingRelationsQuery(noteId));

        if (result.IsFailed)
        {
            throw new ApplicationException("GrpcService result failed");
        }

        var response = new GetIncomingRelationsResponse();
        foreach (var incomingNoteId in result.Value.IncomingNoteIds)
        {
            response.NoteIds.Add(new GrpcUuid()
            {
                Value= incomingNoteId.ToString()
            });
        }

        return response;
    }

}
