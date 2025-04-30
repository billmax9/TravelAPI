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

    
    // Returns all trips along with the list of countries associated with each trip.
    // In case of any unexpected exceptions returns 500 Status code with message
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

    // Returns all registered client to trip
    // If trip with id can't be found - 404 Not Found is returned with appropriate message
    // In case of any unexpected exceptions returns 500 Status code with message
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