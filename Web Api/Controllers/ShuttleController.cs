using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using System;

namespace Web_Api.Controllers
{
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
                else
                {
                    return BadRequest("Échec de l'embarquement dans la navette.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'embarquement dans la navette : {ex.Message}");
            }
        }
    }
}
