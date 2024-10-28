using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using System;

namespace Web_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BikeController : ControllerBase
    {
        private readonly ITransportationService _transportationService;

        public BikeController(ITransportationService transportationService)
        {
            _transportationService = transportationService;
        }

        [HttpPost("rent")]
        public IActionResult RentBike(int userId, string bikeId)
        {
            try
            {
                DateTime rentalStartTime;
                bool result = _transportationService.RentBike(userId, bikeId, out rentalStartTime);

                if (result)
                {
                    return Ok(new { Message = "Vélo loué avec succès.", RentalStartTime = rentalStartTime });
                }
                else
                {
                    return BadRequest("Échec de la location du vélo.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la location du vélo : {ex.Message}");
            }
        }

        [HttpPost("end")]
        public IActionResult EndBikeRental(int userId, string bikeId)
        {
            try
            {
                DateTime rentalEndTime = DateTime.Now;
                bool result = _transportationService.EndBikeRental(userId, bikeId, rentalEndTime);

                if (result)
                {
                    return Ok(new { Message = "Location de vélo terminée avec succès.", RentalEndTime = rentalEndTime });
                }
                else
                {
                    return BadRequest("Échec de la fin de la location du vélo.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la fin de la location du vélo : {ex.Message}");
            }
        }
    }
}
