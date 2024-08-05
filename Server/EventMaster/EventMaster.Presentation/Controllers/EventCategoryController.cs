using EventMaster.BLL.Services.Interfaces;
using EventMaster.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EventMaster.Controllers;

[Route("api/eventCategory")]
[ModelValidator]
public class EventCategoryController:Controller
{
    private readonly IEventCategoryService _eventCategoryService;

    public EventCategoryController(IEventCategoryService eventCategoryService)
    {
        _eventCategoryService = eventCategoryService;
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _eventCategoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _eventCategoryService.GetByIdAsync(id, cancellationToken);
        return Ok(category);
    }
}