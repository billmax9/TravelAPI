using Microsoft.Data.SqlClient;
using WebAPI.DTOs.Client;
using WebAPI.Exceptions;
using WebAPI.Utils;

namespace WebAPI.Services.Client;

public class ClientService : IClientService
{
    // Base Service impl
    public async Task<IEnumerable<ClientResponseDto>> FindAllAsync()
    {
        List<ClientResponseDto> clients = new List<ClientResponseDto>();

        string sql = "SELECT * FROM Client";

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clients.Add(new ClientResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdClient")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Telephone")),
                    Pesel = reader.GetString(reader.GetOrdinal("Pesel"))
                });
            }
        }

        return clients;
    }

    public async Task<ClientResponseDto?> FindByIdAsync(long id)
    {
        string sql = "SELECT * FROM Client WHERE IdClient = @clientId";

        ClientResponseDto? client = null;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@clientId", id);
            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                client = new ClientResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdClient")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Telephone")),
                    Pesel = reader.GetString(reader.GetOrdinal("Pesel"))
                };
            }
        }

        return client;
    }

    // Client Service impl
    public async Task<IEnumerable<ClientTripsResponseDto>> FindTripsByClientIdAsync(long id)
    {
        ClientResponseDto? client = await FindByIdAsync(id);
        if (client == null)
            throw new EntityNotFoundException($"Client with id {id} was not found");

        List<ClientTripsResponseDto> clientTrips = new List<ClientTripsResponseDto>();

        string sql = """
                     SELECT
                         t.IdTrip, Name, Description, DateFrom, DateTo, MaxPeople,
                         RegisteredAt, PaymentDate FROM
                     Trip t
                     INNER JOIN Client_Trip ct ON ct.IdTrip = t.IdTrip AND ct.IdClient = @clientId;
                     """;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@clientId", id);
            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                clientTrips.Add(new ClientTripsResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                    RegisteredAt = reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                    PaymentDate = reader.GetInt32(reader.GetOrdinal("PaymentDate")),
                });
            }
        }

        return clientTrips;
    }

    public async Task<ClientResponseDto?> FindByEmailAsync(string email)
    {
        string sql = "SELECT * FROM Client WHERE Email = @email";

        ClientResponseDto? client = null;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@email", email);
            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                client = new ClientResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdClient")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Telephone")),
                    Pesel = reader.GetString(reader.GetOrdinal("Pesel"))
                };
            }
        }

        return client;
    }

    public async Task<ClientResponseDto?> FindByPeselAsync(string pesel)
    {
        string sql = "SELECT * FROM Client WHERE Email = @pesel";

        ClientResponseDto? client = null;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@pesel", pesel);
            await connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                client = new ClientResponseDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("IdClient")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Telephone")),
                    Pesel = reader.GetString(reader.GetOrdinal("Pesel"))
                };
            }
        }

        return client;
    }

    public async Task<int> CreateAsync(ClientRequestDto requestDto)
    {
        ClientResponseDto? duplicate = await FindByEmailAsync(requestDto.Email);
        if (duplicate != null)
            throw new EntityAlreadyExistsException($"Client with email {requestDto.Email} already exists");
        duplicate = await FindByPeselAsync(requestDto.Pesel);
        if (duplicate != null)
            throw new EntityAlreadyExistsException($"Client with pesel {requestDto.Pesel} already exists");

        string sql = """
                     INSERT INTO Client(FirstName, LastName, Email, Telephone, Pesel)
                     VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Pesel);
                     SELECT SCOPE_IDENTITY();
                     """;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@FirstName", requestDto.FirstName);
            command.Parameters.AddWithValue("@LastName", requestDto.LastName);
            command.Parameters.AddWithValue("@Email", requestDto.Email);
            command.Parameters.AddWithValue("@PhoneNumber", requestDto.PhoneNumber);
            command.Parameters.AddWithValue("@Pesel", requestDto.Pesel);
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}