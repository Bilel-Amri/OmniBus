using System;

namespace OmniBus.Application.DTOs
{
    public class BookingDto : BaseDto
    {
        public Guid UserId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int? PaymentId { get; set; }
    }

    public class CreateBookingDto
    {
        public Guid UserId { get; set; }
        public int ScheduleId { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();
    }

    public class BookingResponseDto : BaseDto
    {
        public Guid UserId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
        public string? BookingReference { get; set; }
        public RouteDto? Route { get; set; }
        public ScheduleDto? Schedule { get; set; }
    }
}