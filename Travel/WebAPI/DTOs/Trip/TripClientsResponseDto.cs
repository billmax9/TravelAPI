using WebAPI.DTOs.Client;

namespace WebAPI.DTOs.Trip;

public class TripClientsResponseDto : ClientResponseDto
{
    public int RegisteredAt { get; set; }

    public int? PaymentDate { get; set; }
}