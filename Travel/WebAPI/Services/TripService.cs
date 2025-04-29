using Microsoft.Data.SqlClient;
using WebAPI.DTOs;
using WebAPI.Utils;

namespace WebAPI.Services;

public class TripService : ITripService
{
    public async Task<IEnumerable<TripDto>> findAllAsync()
    {
        List<TripDto> trips = new List<TripDto>();
        string sqlText = "SELECT * FROM TRIP";

        using (SqlConnection connection = new SqlConnection(Constants.DOCKER_CONNECTION_STRING))
        using (SqlCommand sqlCommand = new SqlCommand(sqlText, connection))
        {
            await connection.OpenAsync();

            using (var reader = await sqlCommand.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new TripDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople"))
                    });
                }
            }
        }

        return trips;
    }

    public async Task<TripDto?> findByIdAsycn(long id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> deleteByIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}