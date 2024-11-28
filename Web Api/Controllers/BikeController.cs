using BLL.Services;
using Microsoft.AspNetCore.Mvc;

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
                return BadRequest(new { Message = "Échec de la location du vélo." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de la location.", Error = ex.Message });
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
                return BadRequest(new { Message = "Échec de la fin de la location du vélo." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de la fin de location.", Error = ex.Message });
            }
        }
    }
}