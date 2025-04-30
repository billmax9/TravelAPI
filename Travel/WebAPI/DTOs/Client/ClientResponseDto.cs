namespace WebAPI.DTOs.Client;

public class ClientResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Pesel { get; set; }
    
    
    public override bool Equals(object? client)
    {
        if (client is not ClientResponseDto other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
    
}