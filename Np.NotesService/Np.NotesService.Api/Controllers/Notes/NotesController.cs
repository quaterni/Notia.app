using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Notes.AddNote;
using Np.NotesService.Application.Notes.RemoveNote;
using Np.NotesService.Application.Notes.GetNote;
using Np.NotesService.Application.Notes.GetNotesFromRoot;
using Np.NotesService.Application.Notes.UpdateNote;
using Np.NotesService.Application.Relations.GetIncomingRelations;
using Np.NotesService.Application.Relations.GetOutgoingRelations;

namespace Np.NotesService.Api.Controllers.Notes;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public NotesController(
        ISqlConnectionFactory sqlConnectionFactory,
        ISender sender,
        IMapper mapper)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("root")]
    public async Task<ActionResult<List<NoteItem>>> GetNotesFromRoot()
    {
        var response = await _sender.Send(new GetNotesFromRootQuery());

        var noteResponses = response.Value.Notes.Select(n=> new NoteItem(n.Title, n.Id)).ToList();

        return Ok(noteResponses);
    }

    [HttpGet("{id:guid}", Name=nameof(GetNote))]
    public async Task<ActionResult<GetNoteResponse>> GetNote(Guid id)
    {
        var result = await _sender.Send(new GetNoteQuery(id));

        if (result.IsFailed)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateNote([FromBody] UpdateNoteRequest request, Guid id)
    {
        var result = await _sender.Send(new UpdateNoteCommand(request.Data, id));
        if (result.IsFailed && result.Message.Equals(Application.Notes.UpdateNote.Errors.NotFound))
        {
            return NotFound(result.Message);
        }
        if (result.IsFailed)
        {
            return StatusCode(500);
        }

        return Ok();
    }


    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteNote(Guid id)
    {
        var result = await _sender.Send(new RemoveNoteCommand(id));

        if (result.IsFailed)
        {
            return StatusCode(500);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> CreateNote([FromBody] AddNoteRequest request)
    {
        var result = await _sender.Send(_mapper.Map<AddNoteCommand>(request));

        if (result.IsFailed)
        {
            return StatusCode(500);
        }

        return CreatedAtRoute(nameof(GetNote), new { id = result.Value }, result.Value);
    }

    [HttpGet("{outgoingNoteId:guid}/outgoings")]
    public async Task<ActionResult> GetOutgoingRelations(Guid outgoingNoteId)
    {
        var result = await _sender.Send(new GetOutgoingRelationsQuery(outgoingNoteId));

        return Ok(result.Value.Relations.Select(
            r=> new RelationItem(
                r.Id, 
                new NoteItem(r.OutgoingItem.Title, r.OutgoingItem.Id), 
                new NoteItem(r.IncomingNote.Title, r.IncomingNote.Id))));
    }

    [HttpGet("{incomingNoteId:guid}/incomings")]
    public async Task<ActionResult> GetIncomingRelations(Guid incomingNoteId)
    {
        var result = await _sender.Send(new GetIncomingRelationsQuery(incomingNoteId));

        return Ok(result.Value.Relations.Select(
            r => new RelationItem(
                r.Id,
                new NoteItem(r.OutgoingItem.Title, r.OutgoingItem.Id),
                new NoteItem(r.IncomingNote.Title, r.IncomingNote.Id))));
    }

}

