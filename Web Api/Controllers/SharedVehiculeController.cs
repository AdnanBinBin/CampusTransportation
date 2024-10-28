using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using System;

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
                else
                {
                    return BadRequest("Échec de la création du trajet en véhicule partagé.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la création du trajet : {ex.Message}");
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
                else
                {
                    return BadRequest("Échec de la location du véhicule partagé.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la location du véhicule partagé : {ex.Message}");
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
                else
                {
                    return BadRequest("Échec de la fin du trajet en véhicule partagé.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la fin du trajet : {ex.Message}");
            }
        }
    }
}
