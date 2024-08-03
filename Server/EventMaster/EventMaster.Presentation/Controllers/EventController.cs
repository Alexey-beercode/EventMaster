using EventMaster.BLL.DTOs.Requests.Event;
using EventMaster.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Controllers;

[Route("api/event")]
public class EventController:Controller
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet("getByParams")]
    public async Task<IActionResult> GetByParamsAsync([FromBody] EventFilterDto eventFilterDto, CancellationToken cancellationToken = default)
    {
        var events = await _eventService.GetFilteredEventsAsync(eventFilterDto, cancellationToken);
        return Ok(events);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var eventById = await _eventService.GetByIdAsync(id, cancellationToken);
        return Ok(eventById);
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var events = await _eventService.GetAllAsync(cancellationToken);
        return Ok(events);
    }
    
}