using System.Threading.Tasks;
using OmniBus.Application.DTOs;

namespace OmniBus.Application.Interfaces
{
    public interface ITrackingService
    {
        Task<bool> UpdateBusLocationAsync(int busId, double latitude, double longitude);
        Task<BusLocationDto?> GetBusLocationAsync(int busId);
        Task<List<BusLocationDto>> GetAllBusLocationsAsync();
        Task<bool> StartTripTrackingAsync(int tripId, int busId);
        Task<bool> StopTripTrackingAsync(int tripId);
        Task<double> CalculateEstimatedArrivalAsync(int busId, int stopId);
    }
}