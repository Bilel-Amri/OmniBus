namespace OmniBus.Domain.Enums;

/// <summary>
/// Represents the type of bus service
/// </summary>
public enum BusType
{
    /// <summary>
    /// City bus service within a single city
    /// </summary>
    City = 0,
    
    /// <summary>
    /// Intercity bus service between different cities
    /// </summary>
    Intercity = 1,
    
    /// <summary>
    /// Express service with limited stops
    /// </summary>
    Express = 2,
    
    /// <summary>
    /// Shuttle service for specific routes
    /// </summary>
    Shuttle = 3
}

/// <summary>
/// Current status of a bus
/// </summary>
public enum BusStatus
{
    /// <summary>
    /// Bus is active and running
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Bus is in maintenance
    /// </summary>
    Maintenance = 1,
    
    /// <summary>
    /// Bus is temporarily out of service
    /// </summary>
    OutOfService = 2,
    
    /// <summary>
    /// Bus is retired/disposed
    /// </summary>
    Retired = 3
}

/// <summary>
/// Status of a ticket
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket is reserved but not yet paid
    /// </summary>
    Reserved = 0,
    
    /// <summary>
    /// Ticket is confirmed and paid
    /// </summary>
    Booked = 1,
    
    /// <summary>
    /// Ticket has been used (passenger boarded)
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Ticket was cancelled
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Ticket expired (not used and past departure)
    /// </summary>
    Expired = 4
}

/// <summary>
/// Status of a payment
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment is pending
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Payment was successful
    /// </summary>
    Completed = 1,
    
    /// <summary>
    /// Payment failed
    /// </summary>
    Failed = 2,
    
    /// <summary>
    /// Payment was refunded
    /// </summary>
    Refunded = 3,
    
    /// <summary>
    /// Payment was cancelled
    /// </summary>
    Cancelled = 4
}

/// <summary>
/// Status of a seat lock (for concurrent booking)
/// </summary>
public enum SeatLockStatus
{
    /// <summary>
    /// Seat is available for booking
    /// </summary>
    Available = 0,
    
    /// <summary>
    /// Seat is temporarily locked for a user
    /// </summary>
    Locked = 1,
    
    /// <summary>
    /// Seat is booked
    /// </summary>
    Booked = 2
}

/// <summary>
/// Status of a schedule/trip
/// </summary>
public enum ScheduleStatus
{
    /// <summary>
    /// Trip is scheduled and active
    /// </summary>
    Scheduled = 0,
    
    /// <summary>
    /// Bus has departed
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Trip is completed
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Trip was cancelled
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Trip is delayed
    /// </summary>
    Delayed = 4
}

/// <summary>
/// User role in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular passenger
    /// </summary>
    Passenger = 0,
    
    /// <summary>
    /// Bus driver
    /// </summary>
    Driver = 1,
    
    /// <summary>
    /// Administrator
    /// </summary>
    Admin = 2
}

/// <summary>
/// Booking status
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking is pending payment
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Booking is confirmed and paid
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// Booking was cancelled
    /// </summary>
    Cancelled = 2,
    
    /// <summary>
    /// Booking is completed (trip finished)
    /// </summary>
    Completed = 3
}

/// <summary>
/// Driver status
/// </summary>
public enum DriverStatus
{
    /// <summary>
    /// Driver is active and available
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Driver is on leave
    /// </summary>
    OnLeave = 1,
    
    /// <summary>
    /// Driver is suspended
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// Driver is terminated
    /// </summary>
    Terminated = 3
}

/// <summary>
/// Payment gateway providers for Tunisia
/// </summary>
public enum PaymentGatewayType
{
    /// <summary>
    /// D17 Payment Gateway
    /// </summary>
    D17 = 0,
    
    /// <summary>
    /// Flouci Mobile Payment
    /// </summary>
    Flouci = 1,
    
    /// <summary>
    /// Konnect Payment Platform
    /// </summary>
    Konnect = 2
}
