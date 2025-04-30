using Microsoft.Data.SqlClient;
using WebAPI.DTOs.Country;
using WebAPI.DTOs.Trip;
using WebAPI.Exceptions;
using WebAPI.Utils;

namespace WebAPI.Services.Trip;

public class TripService : ITripService
{
    // Base Service impl
    public async Task<IEnumerable<TripResponseDto>> FindAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<TripResponseDto?> FindByIdAsync(int id)
    {
        TripResponseDto? trip = null;

        string sql = "SELECT * FROM Trip WHERE IdTrip = @TripId";

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@TripId", id);
            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                trip = new TripResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople"))
                };
            }
        }

        return trip;
    }

    // Trip Service Impl
    public async Task<IEnumerable<TripCountriesResponseDto>> FindAllTripCountriesAsync()
    {
        Dictionary<int, TripCountriesResponseDto> tripsDict = new Dictionary<int, TripCountriesResponseDto>();

        string sql = """"
                        SELECT
                            t.IdTrip, t.Name,
                            Description, DateFrom, DateTo, MaxPeople,
                            c.IdCountry, c.Name as 'CountryName'
                        FROM Trip t
                        INNER JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
                        INNER JOIN Country c ON ct.IdCountry = c.IdCountry;
                     """";

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync();

            using (var reader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int tripId = reader.GetInt32(reader.GetOrdinal("IdTrip"));

                    CountryResponseDto countryResponseDto = new CountryResponseDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("IdCountry")),
                        Name = reader.GetString(reader.GetOrdinal("CountryName"))
                    };

                    if (!tripsDict.ContainsKey(tripId))
                    {
                        tripsDict.Add(tripId, new TripCountriesResponseDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = new List<CountryResponseDto> { countryResponseDto }
                        });
                    }
                    else tripsDict[tripId].Countries.Add(countryResponseDto);
                }
            }
        }

        return tripsDict.Values;
    }

    public async Task<IEnumerable<TripClientsResponseDto>> FindTripClientsByTripIdAsync(int id)
    {
        TripResponseDto? trip = await FindByIdAsync(id);
        if (trip == null)
            throw new EntityNotFoundException($"Trip with id {id} was not found!");

        List<TripClientsResponseDto> tripClients = new List<TripClientsResponseDto>();

        string sql = """
                     SELECT
                         c.IdClient, FirstName, LastName, Email, Telephone, Pesel,
                         RegisteredAt, PaymentDate
                     FROM Trip t
                     INNER JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
                     INNER JOIN Client c ON ct.IdClient = c.IdClient
                     WHERE t.IdTrip = @TripId;
                     """;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@TripId", id);

            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tripClients.Add(new TripClientsResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdClient")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Telephone")),
                    Pesel = reader.GetString(reader.GetOrdinal("Pesel")),
                    RegisteredAt = reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                    PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate")) 
                        ? null 
                        : reader.GetInt32(reader.GetOrdinal("PaymentDate"))
                });
            }
        }

        return tripClients;
    }
}