using EventMaster.BLL.DTOs.Requests.Participant;
using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Areas.Admin.Controllers;


[Route("api/admin/participant")]
[Authorize(Policy = "AdminArea")]
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