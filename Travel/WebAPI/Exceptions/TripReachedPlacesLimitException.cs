namespace WebAPI.Exceptions;

public class TripReachedPlacesLimitException(string? message) : Exception(message)
{
    
}