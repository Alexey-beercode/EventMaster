using EventMaster.BLL.DTOs.Requests.Participant;
using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Controllers;

[Route("api/participant")]
public class ParticipantController:Controller
{
    private readonly IParticipantService _participantService;

    public ParticipantController(IParticipantService participantService)
    {
        _participantService = participantService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateParticipantDTO participantDto,CancellationToken cancellationToken=default)
    {
        await _participantService.CreateAsync(participantDto, cancellationToken);
        return Ok();
    }

    [Authorize(Policy = "AdminArea")]
    [HttpGet("getByEvent/{eventId}")]
    public async Task<IActionResult> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var participantsByEventId = await _participantService.GetByEventIdAsync(eventId, cancellationToken);
        return Ok(participantsByEventId);
    }

    [Authorize]
    [HttpGet("getByUser/{userId}")]
    public async Task<IActionResult> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var participantByUserId = await _participantService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(participantByUserId);
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _participantService.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}