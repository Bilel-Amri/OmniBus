import * as signalR from '@microsoft/signalr'

const API_BASE_URL = import.meta.env.VITE_API_URL || ''
// strip trailing /api if present so hubs resolve to the host root
const HUB_BASE_URL = API_BASE_URL.replace(/\/api\/?$/, '') || ''

class SignalRService {
  private trackingConnection: signalR.HubConnection | null = null
  private bookingConnection: signalR.HubConnection | null = null
  private isConnecting = false
  
  // Tracking hub methods
  async connectToTrackingHub() {
    // Already connected
    if (this.trackingConnection?.state === signalR.HubConnectionState.Connected) {
      return this.trackingConnection
    }
    
    // Prevent multiple simultaneous connection attempts (React StrictMode)
    if (this.isConnecting) {
      // Wait for existing connection attempt
      await new Promise(resolve => setTimeout(resolve, 100))
      if (this.trackingConnection?.state === signalR.HubConnectionState.Connected) {
        return this.trackingConnection
      }
    }
    
    this.isConnecting = true
    
    try {
      // Stop any existing connection first
      if (this.trackingConnection) {
        try {
          await this.trackingConnection.stop()
        } catch {
          // Ignore stop errors
        }
      }
      
      this.trackingConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${HUB_BASE_URL}/hubs/tracking`)
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.None)  // Suppress SignalR internal logs
        .build()
      
      await this.trackingConnection.start()
      console.log('Connected to Tracking Hub')
      
      return this.trackingConnection
    } catch (err: any) {
      // Suppress "connection stopped during negotiation" - expected in React StrictMode
      if (err?.message?.includes('stopped during negotiation')) {
        return null
      }
      throw err
    } finally {
      this.isConnecting = false
    }
  }
  
  async joinRoute(routeId: string) {
    await this.connectToTrackingHub()
    await this.trackingConnection?.invoke('JoinRoute', routeId)
  }
  
  async leaveRoute(routeId: string) {
    await this.trackingConnection?.invoke('LeaveRoute', routeId)
  }
  
  async joinSchedule(scheduleId: string) {
    await this.connectToTrackingHub()
    await this.trackingConnection?.invoke('JoinSchedule', scheduleId)
  }
  
  async leaveSchedule(scheduleId: string) {
    await this.trackingConnection?.invoke('LeaveSchedule', scheduleId)
  }
  
  async joinAdmin() {
    await this.connectToTrackingHub()
    // Register handler before invoking to avoid warning
    this.trackingConnection?.on('JoinedAdmin', () => {
      console.log('Joined admin group')
    })
    await this.trackingConnection?.invoke('JoinAdmin')
  }
  
  // Booking hub methods
  async connectToBookingHub() {
    if (this.bookingConnection?.state === signalR.HubConnectionState.Connected) {
      return this.bookingConnection
    }
    
    try {
      this.bookingConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${HUB_BASE_URL}/hubs/booking`)
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.None)  // Suppress SignalR internal logs
        .build()
      
      await this.bookingConnection.start()
      console.log('Connected to Booking Hub')
      
      return this.bookingConnection
    } catch (err: any) {
      // Suppress "connection stopped during negotiation" - expected in React StrictMode
      if (err?.message?.includes('stopped during negotiation')) {
        return null
      }
      throw err
    }
  }
  
  async joinBookingSchedule(scheduleId: string) {
    await this.connectToBookingHub()
    await this.bookingConnection?.invoke('JoinSchedule', scheduleId)
  }
  
  async leaveBookingSchedule(scheduleId: string) {
    await this.bookingConnection?.invoke('LeaveSchedule', scheduleId)
  }
  
  // Event listeners
  onBusLocationUpdated(callback: (scheduleId: string, location: { latitude: number; longitude: number; timestamp?: Date }) => void) {
    this.trackingConnection?.on('BusLocationUpdated', callback)
  }
  
  onScheduleStatusChanged(callback: (scheduleId: string, status: string, reason?: string) => void) {
    this.trackingConnection?.on('ScheduleStatusChanged', callback)
  }
  
  onDelayNotification(callback: (data: { scheduleId: string; delayMinutes: number; reason: string; newDepartureTime: Date }) => void) {
    this.trackingConnection?.on('DelayNotification', callback)
  }
  
  onSeatLocked(callback: (data: { scheduleId: string; seatNumber: number; userId: string; expiresAt: Date }) => void) {
    this.bookingConnection?.on('SeatLocked', callback)
  }
  
  onSeatLockReleased(callback: (data: { scheduleId: string; seatNumber: number }) => void) {
    this.bookingConnection?.on('SeatLockReleased', callback)
  }
  
  onSeatBooked(callback: (data: { scheduleId: string; seatNumber: number; userId: string; bookingReference: string }) => void) {
    this.bookingConnection?.on('SeatBooked', callback)
  }
  
  onAvailabilityChanged(callback: (data: { scheduleId: string; availableSeats: number }) => void) {
    this.bookingConnection?.on('AvailabilityChanged', callback)
  }
  
  // Cleanup
  removeAllListeners() {
    this.trackingConnection?.off('BusLocationUpdated')
    this.trackingConnection?.off('ScheduleStatusChanged')
    this.trackingConnection?.off('DelayNotification')
    this.bookingConnection?.off('SeatLocked')
    this.bookingConnection?.off('SeatLockReleased')
    this.bookingConnection?.off('SeatBooked')
    this.bookingConnection?.off('AvailabilityChanged')
  }
  
  async disconnect() {
    this.removeAllListeners()
    await this.trackingConnection?.stop()
    await this.bookingConnection?.stop()
    this.trackingConnection = null
    this.bookingConnection = null
  }
}

export const signalRService = new SignalRService()
