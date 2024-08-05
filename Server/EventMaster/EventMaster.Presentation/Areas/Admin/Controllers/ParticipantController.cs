using EventMaster.BLL.Services.Interfaces;
using EventMaster.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Areas.Admin.Controllers;

[Area("admin")]
[Route("api/participant")]
[Authorize(Policy = "AdminArea")]
[ModelValidator]
public class ParticipantController:Controller
{
    private readonly IParticipantService _participantService;

    public ParticipantController(IParticipantService participantService)
    {
        _participantService = participantService;
    }
    
    [HttpGet("getByEvent/{eventId}")]
    public async Task<IActionResult> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var participantsByEventId = await _participantService.GetByEventIdAsync(eventId, cancellationToken);
        return Ok(participantsByEventId);
    }
    
}