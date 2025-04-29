using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;

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
        IEnumerable<TripDto> trips = await _tripService.findAllAsync();
        return Ok(trips);
    }
    
}