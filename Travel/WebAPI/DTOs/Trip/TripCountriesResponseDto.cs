using WebAPI.DTOs.Country;

namespace WebAPI.DTOs.Trip;

public class TripCountriesResponseDto : TripResponseDto
{
    public List<CountryResponseDto> Countries { get; set; }
}