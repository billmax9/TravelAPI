using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Client;
using WebAPI.Exceptions;
using WebAPI.Services.Client;

namespace WebAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }


    // Returns all information about clients stored in database.
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            IEnumerable<ClientResponseDto> clients = await _clientService.FindAllAsync();
            return Ok(clients);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    // Searching and returns client by specified id, in case if client does not exist in database, returns 404 NotFound with appropriate message
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            ClientResponseDto? client = await _clientService.FindByIdAsync(id);
            return client == null ? NotFound($"Client with id {id} was not found") : Ok(client);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    // Searching and returns client by specified pesel, in case if client with provided pesel does not exist in database, returns 404 NotFound with appropriate message
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpGet]
    [Route("pesel/{pesel}")]
    public async Task<IActionResult> GetByPesel(string pesel)
    {
        try
        {
            ClientResponseDto? client = await _clientService.FindByPeselAsync(pesel);
            return client == null
                ? NotFound(new { Message = $"Client with pesel {pesel} was not found!" })
                : Ok(client);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    // Searching and returns client by specified email, in case if client with provided email does not exist in database, returns 404 NotFound with appropriate message
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpGet]
    [Route("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            ClientResponseDto? client = await _clientService.FindByEmailAsync(email);
            return client == null
                ? NotFound(new { Message = $"Client with email {email} was not found!" })
                : Ok(client);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }
    
    // Returns all client's trips that client is registered to.
    // If client with provided id does not exist in database or client is not registered to any trip - 404 NotFound is returned (with appropriate message) 
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpGet]
    [Route("{id:int}/trips")]
    public async Task<IActionResult> GetClientsTripsByClientId(int id)
    {
        try
        {
            IEnumerable<ClientTripsResponseDto> clientTrips = await _clientService.FindTripsByClientIdAsync(id);
            return clientTrips.Any()
                ? Ok(clientTrips)
                : NotFound(new { Message = $"Client with id {id} has no trips" });
        }
        catch (EntityNotFoundException e)
        {
            return NotFound(new { e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    
    // Attempts to create a new client based on provided data
    // In case of any validation errors - returns 400 BadRequest or in case if resource already exists - returns 409 Conflict
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpPost]
    public async Task<IActionResult> CreateClient(ClientRequestDto requestDto)
    {
        try
        {
            int id = await _clientService.CreateClientAsync(requestDto);
            return Created($"/api/clients/{id}", new { id });
        }
        catch (EntityAlreadyExistsException e)
        {
            return Conflict(new { e.Message });
        }
        catch (ValidationException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    // Registers a client to a specific trip.
    // Returns 409 Conflict if the client is already registered or if there are no available places for the trip.
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpPut]
    [Route("{clientId:int}/trips/{tripId:int}")]
    public async Task<IActionResult> RegisterClientToTrip(int clientId, int tripId)
    {
        try
        {
            bool result = await _clientService.RegisterClientTripAsync(clientId, tripId);

            return result == false
                ? StatusCode(500, new { Message = "Something went wrong..." })
                : Created($"/api/clients/{clientId}/trips", new { Message = "Client was successfully registered" });
        }
        catch (ClientAlreadyRegisteredException e)
        {
            return Conflict(new { e.Message });
        }
        catch (TripReachedPlacesLimitException e)
        {
            return Conflict(new { e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }
    
    
    // Tries to remove a client's registration from a trip
    // Returns 404 NotFound if trip or client with provided id was not found or if client does is not registered to trip with provided id
    // In case of any unexpected exceptions returns 500 Status code with message
    [HttpDelete]
    [Route("{clientId:int}/trips/{tripId:int}")]
    public async Task<IActionResult> DeleteRegistration(int clientId, int tripId)
    {
        try
        {
            bool result = await _clientService.DeleteTripRegistrationAsync(clientId, tripId);

            return result == false
                ? StatusCode(500, new { Message = "Something went wrong..." })
                : Ok(new { Message = "Registration was successfully deleted!" });
        }
        catch (EntityNotFoundException e)
        {
            return NotFound(new { e.Message });
        }
        catch (ClientNotRegisteredException e)
        {
            return NotFound(new { e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }
}