using AutoMapper;
using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.Event;

public class CreateEventUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEventUseCase(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task ExecuteAsync(CreateEventDTO createEventDto, CancellationToken cancellationToken = default)
    {
        var newEvent = _mapper.Map<Domain.Entities.Event>(createEventDto);

        if (createEventDto.Image != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await createEventDto.Image.CopyToAsync(memoryStream, cancellationToken);
                newEvent.Image = memoryStream.ToArray();
            }
        }

        await _unitOfWork.Events.CreateAsync(newEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}