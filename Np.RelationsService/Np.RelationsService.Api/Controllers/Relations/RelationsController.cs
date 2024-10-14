using MediatR;
using Microsoft.AspNetCore.Mvc;
using Np.RelationsService.Application.Relations.AddRelation;
using Np.RelationsService.Application.Relations.RemoveRelation;

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
    public async Task<ActionResult> AddRelation([FromBody]AddRelationRequest request)
    {
        var result = await _sender.Send(new AddRelationCommand(request.OutgoingNoteId, request.IncomingNoteId));

        if (result.IsFailed)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveRelation(Guid id)
    {
        var result = await _sender.Send(new RemoveRelationCommand(id));

        return Ok();
    }
}
