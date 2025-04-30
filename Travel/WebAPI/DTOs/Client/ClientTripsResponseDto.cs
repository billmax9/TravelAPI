using WebAPI.DTOs.Trip;

namespace WebAPI.DTOs.Client;

public class ClientTripsResponseDto : ClientResponseDto
{

    public List<ClientTripResponseDto> Trips { get; set; }
    
}

public class ClientTripResponseDto : TripResponseDto
{
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}