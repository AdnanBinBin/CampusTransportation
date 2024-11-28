using BLL.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ShuttleController : ControllerBase
{
    private readonly ITransportationService _transportationService;
    public ShuttleController(ITransportationService transportationService)
    {
        _transportationService = transportationService;
    }

    [HttpPost("board")]
    public IActionResult BoardShuttle(int userId, string shuttleId)
    {
        try
        {
            bool result = _transportationService.BoardShuttle(userId, shuttleId);
            if (result)
            {
                return Ok(new { Message = "Embarquement dans la navette réussi." });
            }
            return BadRequest(new { Message = "Échec de l'embarquement dans la navette." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = "Échec de l'embarquement dans la navette", Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de l'embarquement.", Error = ex.Message });
        }
    }
}