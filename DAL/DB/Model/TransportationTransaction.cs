﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DB.Model
{
    public class TransportationTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ShuttleId { get; set; }      // Nullable
        public string? BikeId { get; set; }         // Nullable
        public string? SharedVehiculeId { get; set; }  // Nullable
        public DateTime Date { get; set; }
        public DateTime? RentalStartTime { get; set; }
        public DateTime? RentalEndTime { get; set; }
    }
}