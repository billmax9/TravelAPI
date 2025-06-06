using WebAPI.DTOs.Trip;

namespace WebAPI.Services.Trip;

public interface ITripService : IBaseService<TripResponseDto>
{
    Task<IEnumerable<TripCountriesResponseDto>> FindAllTripCountriesAsync();

    Task<IEnumerable<TripClientsResponseDto>> FindTripClientsByTripIdAsync(int id);
}