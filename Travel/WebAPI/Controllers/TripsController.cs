using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Trip;
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
        IEnumerable<TripCountriesResponseDto> trips = await _tripService.FindAllTripCountriesAsync();
        return Ok(trips);
    }
    
}