using EventMaster.Domain.Models;

namespace EventMaster.BLL.DTOs.Implementations.BaseDTOs;

public class EventBaseDTO<T>:BaseValidationModel<T>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Location Location { get; set; }
    public int MaxParticipants { get; set; }
    public Guid CategoryId { get; set; }
}
