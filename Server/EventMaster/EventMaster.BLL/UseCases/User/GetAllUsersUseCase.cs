using AutoMapper;
using EventMaster.BLL.DTOs.Responses.User;
using EventMaster.DAL.Infrastructure;

namespace EventMaster.BLL.UseCases.User;

public class GetAllUsersUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IEnumerable<UserResponseDTO>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
    }
}