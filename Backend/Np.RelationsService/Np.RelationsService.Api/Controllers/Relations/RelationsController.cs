using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Np.RelationsService.Application.Relations.AddRelation;
using Np.RelationsService.Application.Relations.GetRelationById;
using Np.RelationsService.Application.Relations.GetRelationByNotes;
using Np.RelationsService.Application.Relations.RemoveRelationById;
using Np.RelationsService.Application.Relations.RemoveRelationByNotes;
using System.Security.Claims;

namespace Np.RelationsService.Api.Controllers.Relations;

[Route("api/[controller]")]
[Controller]
public class RelationsController : ControllerBase
{
    private readonly ISender _sender;

    public RelationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> AddRelation([FromBody]RelationRequest request, CancellationToken cancellationToken)
    {
        var identityId = GetIdentityId();
        if (identityId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new AddRelationCommand(request.OutgoingNoteId, request.IncomingNoteId, identityId), cancellationToken);
        if (result.IsFailed)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetRelationByNotes([FromBody] RelationRequest request, CancellationToken cancellationToken)
    {
        var identityId = GetIdentityId();
        if (identityId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new GetRelationByNotesQuery(request.IncomingNoteId, request.OutgoingNoteId, identityId), cancellationToken);
        if(result.IsFailed && result.Error.Equals(GetRelationByNotesErrors.NotFound))
        {
            return NotFound();
        }
        if (result.IsFailed && result.Error.Equals(GetRelationByNotesErrors.BadRequest))
        {
            return BadRequest();
        }

        return Ok(result.Value);
    }


    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> GetRelationById(Guid id, CancellationToken cancellationToken)
    {
        var identityId = GetIdentityId();
        if (identityId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new GetRelationByIdQuery(id, identityId), cancellationToken);
        if (result.IsFailed && result.Error.Equals(GetRelationByIdErrors.NotFound))
        { 
            return NotFound();
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> RemoveRelationById(Guid id, CancellationToken cancellationToken)
    {
        var identityId = GetIdentityId();
        if(identityId == null)
        {
            return Unauthorized();
        }
        var result = await _sender.Send(new RemoveRelationByIdCommand(id, identityId), cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> RemoveRelationByNotes([FromBody] RelationRequest request, CancellationToken cancellationToken)
    {
        var identityId = GetIdentityId();
        if (identityId == null)
        {
            return Unauthorized();
        }
        var result = await _sender.Send(new RemoveRelationByNotesCommand(request.IncomingNoteId, request.OutgoingNoteId, identityId), cancellationToken);

        return Ok();
    }

    private string? GetIdentityId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
