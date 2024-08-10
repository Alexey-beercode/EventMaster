using EventMaster.BLL.Exceptions;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Participant;

public class DeleteParticipantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteParticipantUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        var participant = await _unitOfWork.Participants.GetByIdAsync(participantId, cancellationToken) 
                          ?? throw new EntityNotFoundException("Participant", participantId);
        _unitOfWork.Participants.Delete(participant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}