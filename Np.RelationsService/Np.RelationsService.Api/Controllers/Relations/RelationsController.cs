using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Np.RelationsService.Application.Relations.AddRelation;
using Np.RelationsService.Application.Relations.RemoveRelation;
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
    public async Task<ActionResult> AddRelation([FromBody]AddRelationRequest request)
    {
        var identityId = GetIdentityId();
        if (identityId == null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(new AddRelationCommand(request.OutgoingNoteId, request.IncomingNoteId, identityId));
        if (result.IsFailed)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult> RemoveRelation(Guid id)
    {
        var identityId = GetIdentityId();
        if(identityId == null)
        {
            return Unauthorized();
        }
        var result = await _sender.Send(new RemoveRelationCommand(id, identityId));
        return Ok();
    }

    private string? GetIdentityId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
