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
    [Route("{id:int}/trips")]
    public async Task<IActionResult> GetClientsTripsByClientId(int id)
    {
        try
        {
            IEnumerable<ClientTripsResponseDto> clientTrips = await _clientService.FindTripsByClientIdAsync(id);
            return clientTrips.Count() == 0
                ? NotFound(new { Message = $"Client with id {id} has no trips" })
                : Ok(clientTrips);
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
            int id = await _clientService.CreateAsync(requestDto);
            return Created($"/api/clients/{id}", new { id });
        }
        catch (EntityAlreadyExistsException e)
        {
            return BadRequest(new { e.Message });
        }
    }
}