namespace WebAPI.DTOs;

public class ClientDto
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Pesel { get; set; }
    public List<CountryDto> Countries { get; set; }


    public ClientDto(long id, string firstName, string lastName, string email, string phoneNumber, string pesel, List<CountryDto> countries)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Pesel = pesel;
        Countries = countries;
    }
}