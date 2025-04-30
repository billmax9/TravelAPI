using Microsoft.Data.SqlClient;
using WebAPI.DTOs.Client;
using WebAPI.DTOs.Trip;
using WebAPI.Exceptions;
using WebAPI.Services.Trip;
using WebAPI.Utils;

namespace WebAPI.Services.Client;

public class ClientService : IClientService
{
    private readonly ITripService _tripService;

    public ClientService(ITripService tripService)
    {
        _tripService = tripService;
    }


    // Base Service impl
    public async Task<IEnumerable<ClientResponseDto>> FindAllAsync()
    {
        List<ClientResponseDto> clients = new List<ClientResponseDto>();

        // Selects all clients from database
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

    public async Task<ClientResponseDto?> FindByIdAsync(int id)
    {
        // Selects all clients having IdClient = '@ClientId' (id is a primary key - so only zero/one client will be selected)
        string sql = "SELECT * FROM Client WHERE IdClient = @ClientId";

        ClientResponseDto? client = null;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@ClientId", id);
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
    public async Task<IEnumerable<ClientTripsResponseDto>> FindTripsByClientIdAsync(int id)
    {
        ClientResponseDto? client = await FindByIdAsync(id);
        if (client == null)
            throw new EntityNotFoundException($"Client with id {id} was not found");

        List<ClientTripsResponseDto> clientTrips = new List<ClientTripsResponseDto>();


        // Selects trip information for a specific client, identified by @ClientId
        // Joins the 'Client_Trip' table with the 'Trip' table to retrieve details about the trips 
        // the client is registered for, including the registration date and payment date (if paid)
        string sql = """
                     SELECT
                         t.IdTrip, Name, Description, DateFrom, DateTo, MaxPeople,
                         RegisteredAt, PaymentDate FROM
                     Trip t
                     INNER JOIN Client_Trip ct ON ct.IdTrip = t.IdTrip AND ct.IdClient = @ClientId;
                     """;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@ClientId", id);
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
                    PaymentDate = reader.IsDBNull(reader.GetOrdinal("PaymentDate"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("PaymentDate")),
                });
            }
        }

        return clientTrips;
    }

    public async Task<ClientResponseDto?> FindByEmailAsync(string email)
    {
        // Selects all clients that have email = '@Email' (email is unique - so only zero/one client will be selected)
        string sql = "SELECT * FROM Client WHERE email = @Email";

        ClientResponseDto? client = null;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@Email", email);
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
        // Selects all clients that have pesel = '@Pesel' (pesel is unique - so only zero/one client will be selected)
        string sql = "SELECT * FROM Client WHERE pesel = @Pesel";

        ClientResponseDto? client = null;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@Pesel", pesel);
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

    public async Task<int> CreateClientAsync(ClientRequestDto requestDto)
    {
        ClientValidationService.ValidateEmail(requestDto.Email);
        ClientValidationService.ValidatePhoneNumber(requestDto.PhoneNumber);
        ClientValidationService.ValidatePesel(requestDto.Pesel);

        ClientResponseDto? duplicate = await FindByEmailAsync(requestDto.Email);
        if (duplicate != null)
            throw new EntityAlreadyExistsException($"Client with email {requestDto.Email} already exists");
        duplicate = await FindByPeselAsync(requestDto.Pesel);
        if (duplicate != null)
            throw new EntityAlreadyExistsException($"Client with pesel {requestDto.Pesel} already exists");


        // Creates a new client, and selects id of last inserted record
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

    public async Task<bool> RegisterClientTripAsync(int clientId, int tripId)
    {
        ClientResponseDto? client = await FindByIdAsync(clientId);
        if (client == null)
            throw new EntityNotFoundException($"Client with id {clientId} was not found!");

        // Automatically checks if trip with id 'tripId' exists and throws exception EntityNotFoundException
        List<TripClientsResponseDto> trips = (await _tripService.FindTripClientsByTripIdAsync(tripId)).ToList();

        TripResponseDto? trip = await _tripService.FindByIdAsync(tripId);

        TripClientsResponseDto? alreadyRegisteredClient = trips.Find(c => c.Id == clientId);
        if (alreadyRegisteredClient != null)
            throw new ClientAlreadyRegisteredException(
                $"Client {alreadyRegisteredClient.FirstName} {alreadyRegisteredClient.LastName} already registered to trip {trip?.Name}");

        if (trips.Count == trip?.MaxPeople)
            throw new TripReachedPlacesLimitException(
                $"Trip {trip.Name} has not available places. Limit reached {trips.Count} / {trip.MaxPeople}");

        var now = DateTime.Now.ToString("yyyyMMdd");
        int registrationDate = Convert.ToInt32(now);

        string sql = """
                     INSERT INTO Client_Trip(IdClient, IdTrip, RegisteredAt)
                     VALUES (@ClientId, @TripId, @RegistrationDate);
                     """;

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@ClientId", clientId);
            command.Parameters.AddWithValue("@TripId", tripId);
            command.Parameters.AddWithValue("@RegistrationDate", registrationDate);
            await connection.OpenAsync();

            var affected = await command.ExecuteNonQueryAsync();
            return affected > 0;
        }
    }

    public async Task<bool> DeleteTripRegistrationAsync(int clientId, int tripId)
    {
        ClientResponseDto? client = await FindByIdAsync(clientId);
        if (client == null)
            throw new EntityNotFoundException($"Client with id {clientId} was not found!");

        // Automatically checks if trip with id 'tripId' exists and throws exception EntityNotFoundException
        List<TripClientsResponseDto> trips = (await _tripService.FindTripClientsByTripIdAsync(tripId)).ToList();
        TripResponseDto? trip = await _tripService.FindByIdAsync(tripId);

        bool isClientRegistered = trips.Any(c => c.Id == clientId);
        if (!isClientRegistered)
            throw new ClientNotRegisteredException(
                $"Client {client.FirstName} {client.LastName} is not registered to trip {trip?.Name} ");

        // Deletes the registration of a client from a specific trip 
        string sql = "DELETE FROM Client_Trip WHERE IdClient = @ClientId AND IdTrip = @TripId";

        using (SqlConnection connection = new SqlConnection(Constants.DockerConnectionString))
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@ClientId", clientId);
            command.Parameters.AddWithValue("@TripId", tripId);

            await connection.OpenAsync();

            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
    }
}