using EventMaster.Domain.Models;

namespace EventMaster.BLL.DTOs.Requests.Event;

public class EventFilterDto
{
    public string Name { get; set; }
    public DateTime? Date { get; set; }
    public Location Location { get; set; }
    public Guid? CategoryId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
