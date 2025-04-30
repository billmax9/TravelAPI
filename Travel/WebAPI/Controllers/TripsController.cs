using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Trip;
using WebAPI.Exceptions;
using WebAPI.Services.Trip;

namespace WebAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TripsController : ControllerBase
{

    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            IEnumerable<TripCountriesResponseDto> trips = await _tripService.FindAllTripCountriesAsync();
            return Ok(trips);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    [HttpGet]
    [Route("{id:int}/clients")]
    public async Task<IActionResult> GetAllRegisteredClients(int id)
    {
        try
        {
            IEnumerable<TripClientsResponseDto> tripClients = await _tripService.FindTripClientsByTripIdAsync(id);
            return Ok(tripClients);
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
    
}