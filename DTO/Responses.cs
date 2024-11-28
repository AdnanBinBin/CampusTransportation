using System;
using System.Text.Json.Serialization;

namespace ConsoleApp1.DTOs.Responses
{
    public class RentBikeResponseDto
    {
        public string Message { get; set; }
        public DateTime RentalStartTime { get; set; }
    }

    public class EndBikeRentalResponseDto
    {
        public string Message { get; set; }
        public DateTime RentalEndTime { get; set; }
    }

    public class BoardShuttleResponseDto
    {
        public string Message { get; set; }
        public DateTime BoardingTime { get; set; }
    }

    public class SharedVehicleResponseDto
    {
        public string Message { get; set; }
        public DateTime? RentalStartTime { get; set; }
        public DateTime? RentalEndTime { get; set; }
        public int? DriverId { get; set; }
    }

    public class ApiErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    public class UserTransportationResponseDto
    {
        public string Message { get; set; }
        public List<TransportationTransactionDto> Transactions { get; set; }
    }

    public class UserPaymentResponseDto
    {
        public string Message { get; set; }
        public List<PaymentTransactionDto> Transactions { get; set; }
    }

    public class TransportationTransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ShuttleId { get; set; }
        public string BikeId { get; set; }
        public string SharedVehiculeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? RentalStartTime { get; set; }
        public DateTime? RentalEndTime { get; set; }
    }

    public class PaymentTransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsRefund { get; set; }
    }


}