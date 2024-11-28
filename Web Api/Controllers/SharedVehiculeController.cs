using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedVehicleController : ControllerBase
    {
        private readonly ITransportationService _transportationService;

        public SharedVehicleController(ITransportationService transportationService)
        {
            _transportationService = transportationService;
        }

        [HttpPost("createTrip")]
        public IActionResult CreateSharedVehicleTrip(int driverId, string sharedVehicleId)
        {
            try
            {
                DateTime rentalStartTime = DateTime.Now;
                bool result = _transportationService.CreateSharedVehicleTrip(driverId, sharedVehicleId, rentalStartTime);
                if (result)
                {
                    return Ok(new { Message = "Trajet en véhicule partagé créé avec succès.", RentalStartTime = rentalStartTime });
                }
                return BadRequest(new { Message = "Échec de la création du trajet en véhicule partagé." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de la création du trajet.", Error = ex.Message });
            }
        }

        [HttpPost("rent")]
        public IActionResult RentSharedVehicle(int userId, string sharedVehicleId, int driverId)
        {
            try
            {
                DateTime rentalStartTime;
                bool result = _transportationService.RentSharedVehicle(userId, sharedVehicleId, out rentalStartTime, driverId);
                if (result)
                {
                    return Ok(new { Message = "Véhicule partagé loué avec succès.", RentalStartTime = rentalStartTime });
                }
                return BadRequest(new { Message = "Échec de la location du véhicule partagé." });
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
        public IActionResult EndSharedVehicleRental(string sharedVehicleId, int driverId)
        {
            try
            {
                bool result = _transportationService.EndSharedVehicleRental(sharedVehicleId, driverId);
                if (result)
                {
                    return Ok(new { Message = "Trajet en véhicule partagé terminé avec succès." });
                }
                return BadRequest(new { Message = "Échec de la fin du trajet en véhicule partagé." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de la fin du trajet.", Error = ex.Message });
            }
        }
    }
}