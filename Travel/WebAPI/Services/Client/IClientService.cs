using WebAPI.DTOs.Client;

namespace WebAPI.Services.Client;

public interface IClientService : IBaseService<ClientResponseDto>
{
    Task<IEnumerable<ClientTripsResponseDto>> FindTripsByClientIdAsync(int id);

    Task<ClientResponseDto?> FindByEmailAsync(string email);

    Task<ClientResponseDto?> FindByPeselAsync(string pesel);
    
    Task<int> CreateAsync(ClientRequestDto requestDto);
}