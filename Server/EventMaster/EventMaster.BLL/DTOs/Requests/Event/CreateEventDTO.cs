using EventMaster.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace EventMaster.BLL.DTOs.Requests.Event;

public class CreateEventDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Location Location { get; set; }
    public int MaxParticipants { get; set; }
    public IFormFile Image { get; set; }
    public Guid CategoryId { get; set; }
}