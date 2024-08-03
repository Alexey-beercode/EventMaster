using EventMaster.BLL.DTOs.Requests.Event;
using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Areas.Admin.Controllers;

[Route("api/admin/event")]
[Authorize(Policy = "AdminArea")]
public class EventController:Controller
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateEventDTO createEventDto,
        CancellationToken cancellationToken = default)
    {
        await _eventService.CreateAsync(createEventDto, cancellationToken);
        return Ok();
    }
    
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateEventDTO updateEventDto,CancellationToken cancellationToken=default)
    {
        await _eventService.UpdateAsync(updateEventDto,cancellationToken);
        return Ok();
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _eventService.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}