import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api

// API service functions
export const authService = {
  login: (email: string, password: string) => 
    api.post('/auth/login', { email, password }),
  register: (data: any) => 
    api.post('/auth/register', data),
  getMe: () => 
    api.get('/auth/me'),
  updateProfile: (data: any) => 
    api.put('/auth/profile', data),
  changePassword: (data: any) => 
    api.post('/auth/change-password', data),
}

export const routeService = {
  getAll: () => 
    api.get('/routes'),
  getById: (id: string) => 
    api.get(`/routes/${id}`),
  search: (origin: string, destination: string) => 
    api.get('/routes/search', { params: { origin, destination } }),
  getActive: () => 
    api.get('/routes/active'),
  getCities: () => 
    api.get('/routes/cities'),
  create: (data: any) => 
    api.post('/routes', data),
  update: (id: string, data: any) => 
    api.put(`/routes/${id}`, data),
  delete: (id: string) => 
    api.delete(`/routes/${id}`),
}

export const scheduleService = {
  getAll: () => 
    api.get('/schedules'),
  getById: (id: string) => 
    api.get(`/schedules/${id}`),
  search: (origin: string, destination: string, date: string) => 
    api.get('/schedules/search', { params: { origin, destination, date } }),
  getByDate: (date: string) => 
    api.get(`/schedules/date/${date}`),
  getSeatLayout: (id: string) => 
    api.get(`/schedules/${id}/seats`),
  getActiveWithLocation: () => 
    api.get('/schedules/active'),
  create: (data: any) => 
    api.post('/schedules', data),
  update: (id: string, data: any) => 
    api.put(`/schedules/${id}`, data),
  delete: (id: string) => 
    api.delete(`/schedules/${id}`),
  updateLocation: (id: string, location: { latitude: number; longitude: number }) => 
    api.put(`/schedules/${id}/location`, location),
}

export const ticketService = {
  getAll: () => 
    api.get('/tickets'),
  getById: (id: string) => 
    api.get(`/tickets/${id}`),
  getByReference: (reference: string) => 
    api.get(`/tickets/reference/${reference}`),
  getMyTickets: () => 
    api.get('/tickets/my-tickets'),
  getUpcoming: () => 
    api.get('/tickets/upcoming'),
  getHistory: () => 
    api.get('/tickets/history'),
  lockSeat: (scheduleId: string, seatNumber: number) => 
    api.post('/tickets/lock-seat', { scheduleId, seatNumber }),
  releaseLock: (lockId: string) => 
    api.delete(`/tickets/lock/${lockId}`),
  book: (data: any) => 
    api.post('/tickets/book', data),
  cancel: (id: string, reason: string) => 
    api.post(`/tickets/${id}/cancel`, { reason }),
  getAvailability: (scheduleId: string) => 
    api.get(`/tickets/${scheduleId}/availability`),
  getStats: () => 
    api.get('/tickets/stats'),
}

export const busService = {
  getAll: () => 
    api.get('/buses'),
  getById: (id: string) => 
    api.get(`/buses/${id}`),
  getActive: () => 
    api.get('/buses/active'),
  getByStatus: (status: string) => 
    api.get(`/buses/status/${status}`),
  getByType: (type: string) => 
    api.get(`/buses/type/${type}`),
  create: (data: any) => 
    api.post('/buses', data),
  update: (id: string, data: any) => 
    api.put(`/buses/${id}`, data),
  delete: (id: string) => 
    api.delete(`/buses/${id}`),
}

export const driverService = {
  getTrips: () => 
    api.get('/driver/trips'),
  getToday: () => 
    api.get('/driver/today'),
  updateLocation: (location: { latitude: number; longitude: number }) => 
    api.put('/driver/location', location),
  getScheduleTickets: (scheduleId: string) => 
    api.get(`/driver/schedule/${scheduleId}/tickets`),
  confirmBoarding: (ticketId: string, qrCode?: string) => 
    api.post('/driver/confirm-boarding', { ticketId, qrCode }),
  startTrip: (scheduleId: string) => 
    api.put(`/driver/schedule/${scheduleId}/start`),
  completeTrip: (scheduleId: string) => 
    api.put(`/driver/schedule/${scheduleId}/complete`),
  reportDelay: (scheduleId: string, delayMinutes: number, reason?: string) => 
    api.put(`/driver/schedule/${scheduleId}/delay`, { delayMinutes, reason }),
}

export const paymentService = {
  process: (ticketId: string, paymentMethod: string) => 
    api.post('/payments/process', { ticketId, paymentMethod }),
  getById: (id: string) => 
    api.get(`/payments/${id}`),
  getMyPayments: () => 
    api.get('/payments/my-payments'),
  getStats: () => 
    api.get('/payments/stats'),
}
