using AutoMapper;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Notes.AddNote;
using Np.NotesService.Application.Notes.DeleteNote;
using Np.NotesService.Application.Notes.GetNote;
using Np.NotesService.Application.Notes.UpdateNote;


namespace Np.NotesService.Api.Controllers.Note
{
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

        // TODO: Remove after testing
        [HttpGet]
        public async Task<ActionResult<List<NoteItemResponse>>> GetNotes()
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var noteItems = await connection.QueryAsync<NoteItemResponse>("SELECT title, id FROM notes");

            return Ok(noteItems);
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
        public async Task<ActionResult> Update([FromBody] UpdateNoteRequest request, Guid id)
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
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _sender.Send(new DeleteNoteCommand(id));

            if (result.IsFailed)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] AddNoteRequest request)
        {
            var result = await _sender.Send(_mapper.Map<AddNoteCommand>(request));

            if (result.IsFailed)
            {
                return StatusCode(500);
            }

            return CreatedAtRoute(nameof(GetNote), new { id = result.Value }, result.Value);
        }
    }
}
