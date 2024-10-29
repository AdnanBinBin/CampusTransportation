using System;

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
}