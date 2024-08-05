using EventMaster.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace EventMaster.BLL.DTOs.Implementations.Requests.Event;

public class CreateEventDTO: BaseValidationModel<CreateEventDTO>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Location Location { get; set; }
    public int MaxParticipants { get; set; }
    public IFormFile Image { get; set; }
    public Guid CategoryId { get; set; }
}