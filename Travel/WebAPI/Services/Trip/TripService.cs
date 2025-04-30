using Microsoft.Data.SqlClient;
using WebAPI.DTOs.Country;
using WebAPI.DTOs.Trip;
using WebAPI.Utils;

namespace WebAPI.Services.Trip;

public class TripService : ITripService
{
    // Base Service impl
    public async Task<IEnumerable<TripResponseDto>> FindAllAsync()
    {
        throw new NotImplementedException();

    }

    public async Task<TripResponseDto?> FindByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    // Trip Service Impl
    public async Task<IEnumerable<TripCountriesResponseDto>> FindAllTripCountriesAsync()
    {
       Dictionary<long, TripCountriesResponseDto> tripsDict = new Dictionary<long, TripCountriesResponseDto>();

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

    
}