using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Web_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ITransportationService _transportationService;

        public UserController(ITransportationService transportationService)
        {
            _transportationService = transportationService;
        }

        [HttpGet("transportation/{userId}")]
        public IActionResult GetUserTransportationTransactions(int userId)
        {
            try
            {
                var transactions = _transportationService.GetUserTransportationTransactions(userId);
                if (transactions != null && transactions.Any())
                {
                    return Ok(new { Message = "Transactions récupérées avec succès", Transactions = transactions });
                }
                return NotFound(new { Message = "Aucune transaction trouvée pour cet utilisateur." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de la récupération des transactions.", Error = ex.Message });
            }
        }

        [HttpGet("payment/{userId}")]
        public IActionResult GetUserPaymentTransactions(int userId)
        {
            try
            {
                var transactions = _transportationService.GetUserPaymentTransactions(userId);
                if (transactions != null && transactions.Any())
                {
                    return Ok(new { Message = "Transactions de paiement récupérées avec succès", Transactions = transactions });
                }
                return NotFound(new { Message = "Aucune transaction de paiement trouvée pour cet utilisateur." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur interne s'est produite lors de la récupération des transactions de paiement.", Error = ex.Message });
            }
        }
    }
}