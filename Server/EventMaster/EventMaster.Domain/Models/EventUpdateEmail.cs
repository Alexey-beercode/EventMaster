namespace EventMaster.Domain.Models;

public class EventUpdateEmail
{
    public string ToEmail { get; set; }
    public string Name { get; set; }
    public string EventName { get; set; }
    public string EventDate { get; set; }
    public string EventLocation { get; set; }
}