namespace WebAPI.DTOs.Trip;

public class TripResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    
    public override bool Equals(object? trip)
    {
        if (trip is not TripCountriesResponseDto other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}