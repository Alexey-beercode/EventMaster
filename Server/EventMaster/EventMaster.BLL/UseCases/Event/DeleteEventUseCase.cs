using EventMaster.BLL.Exceptions;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Event;

public class DeleteEventUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventById = await _unitOfWork.Events.GetByIdAsync(eventId, cancellationToken) ??
                        throw new EntityNotFoundException("Event", eventId);

        _unitOfWork.Events.Delete(eventById);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}