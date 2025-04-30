using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<ClientResponseDto> clients = await _clientService.FindAllAsync();
        return Ok(clients);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        ClientResponseDto? client = await _clientService.FindByIdAsync(id);

        return client == null ? NotFound($"Client with id {id} was not found") : Ok(client);
    }

    [HttpGet]
    [Route("pesel/{pesel}")]
    public async Task<IActionResult> GetByPesel(string pesel)
    {
        ClientResponseDto? client = await _clientService.FindByPeselAsync(pesel);
        return client == null
            ? NotFound(new { Message = $"Client with pesel {pesel} was not found!" })
            : Ok(client);
    }

    [HttpGet]
    [Route("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        ClientResponseDto? client = await _clientService.FindByEmailAsync(email);
        return client == null
            ? NotFound(new { Message = $"Client with email {email} was not found!" })
            : Ok(client);
    }

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
    }

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
            return BadRequest(new { e.Message });
        }
    }

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
            return BadRequest(new { e.Message });
        }
        catch (TripReachedPlacesLimitException e)
        {
            Console.WriteLine("Trip is full: " + e.Message);
            return Conflict(new { e.Message });
        }
        // catch (Exception e)
        // {
        //     return StatusCode(500, new { e.Message });
        // }
    }
}