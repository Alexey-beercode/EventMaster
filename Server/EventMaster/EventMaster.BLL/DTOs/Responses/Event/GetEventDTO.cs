using EventMaster.Domain.Models;

namespace EventMaster.BLL.DTOs.Responses.Event;

public class GetEventDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Location Location { get; set; } 
    public int MaxParticipants { get; set; }
    public byte[] Image { get; set; }
    public Guid CategoryId { get; set; }
}