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
    [Route("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        ClientResponseDto? client = await _clientService.FindByIdAsync(id);

        return client == null ? NotFound($"Client with id {id} was not found") : Ok(client);
    }

    [HttpGet]
    [Route("{id:long}/trips")]
    public async Task<IActionResult> GetClientsTripsByClientId(long id)
    {
        ClientTripsResponseDto? clientTrips = await _clientService.FindTripsByClientIdAsync(id);

        if (clientTrips == null)
            return NotFound(new {Message = $"Client with id {id} was not found"});
        if (clientTrips?.Trips.Count == 0)
            return NotFound(new {Message = $"Client with id {id} has no trips", clientTrips});

        return Ok(clientTrips);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient(ClientRequestDto requestDto)
    {
        try
        {
            int id = await _clientService.CreateAsync(requestDto);
            return Created($"/api/clients/{id}", new { id });
        }
        catch (EntityAlreadyExistsException e)
        {
            return BadRequest(new { e.Message });
        }
    }
}