using System;

namespace OmniBus.Application.DTOs
{
    public class DriverDto : BaseDto
    {
        public Guid UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public int? AssignedBusId { get; set; }
    }

    public class UpdateDriverLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}