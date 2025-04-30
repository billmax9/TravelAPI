using WebAPI.DTOs.Trip;

namespace WebAPI.DTOs.Client;

public class ClientTripsResponseDto : TripResponseDto
{
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}