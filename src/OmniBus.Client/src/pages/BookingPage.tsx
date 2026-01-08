import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  Button,
  TextField,
  Alert,
  CircularProgress,
  Divider,
  Stack,
  Chip,
} from '@mui/material'
import { format } from 'date-fns'
import {
  ArrowBack,
  Person,
  Phone,
  CreditCard,
  Warning,
} from '@mui/icons-material'
import { scheduleService, ticketService } from '../services/api'
import LoadingSpinner from '../components/common/LoadingSpinner'

interface Schedule {
  id: string
  departureTime: string
  arrivalTime: string
  availableSeats: number
  basePrice: number
  bus: {
    plateNumber: string
    capacity: number
    seatsPerRow: number
    type: string
    hasAirConditioning: boolean
    hasWifi: boolean
  }
  route: {
    origin: string
    destination: string
    distanceKm: number
  }
}

interface SeatStatus {
  seatNumber: number
  status: 'Available' | 'Locked' | 'Booked'
}

export default function BookingPage() {
  const { scheduleId } = useParams()
  const navigate = useNavigate()
  
  const [schedule, setSchedule] = useState<Schedule | null>(null)
  const [seats, setSeats] = useState<SeatStatus[]>([])
  const [selectedSeat, setSelectedSeat] = useState<number | null>(null)
  const [passengerName, setPassengerName] = useState('')
  const [passengerPhone, setPassengerPhone] = useState('')
  const [loading, setLoading] = useState(true)
  const [bookingLoading, setBookingLoading] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  useEffect(() => {
    fetchSchedule()
  }, [scheduleId])

  const fetchSchedule = async () => {
    try {
      const [scheduleRes, seatsRes] = await Promise.all([
        scheduleService.getById(scheduleId!),
        ticketService.getAvailability(scheduleId!),
      ])
      setSchedule(scheduleRes.data)
      setSeats(seatsRes.data.seats)
    } catch (err: any) {
      if (err.response?.status === 404 || err.response?.status === 400) {
        setError('Schedule not found. Redirecting to search page...')
        setTimeout(() => navigate('/search'), 2000)
      } else {
        setError('Failed to load schedule')
      }
    } finally {
      setLoading(false)
    }
  }

  const handleSeatSelect = (seatNumber: number) => {
    const seat = seats.find(s => s.seatNumber === seatNumber)
    if (seat && seat.status === 'Available') {
      setSelectedSeat(seatNumber === selectedSeat ? null : seatNumber)
    }
  }

  const handleBooking = async () => {
    if (!selectedSeat || !passengerName) {
      setError('Please select a seat and enter passenger name')
      return
    }

    setBookingLoading(true)
    setError('')

    try {
      // Lock the seat first
      await ticketService.lockSeat(scheduleId!, selectedSeat)
      
      // Book the ticket
      await ticketService.book({
        scheduleId,
        seatNumber: selectedSeat,
        passengerName,
        passengerPhone,
      })
      
      setSuccess('Booking confirmed! Redirecting to your tickets...')
      
      // Process payment (simplified - in real app would redirect to payment gateway)
      await new Promise(resolve => setTimeout(resolve, 1500))
      navigate('/my-tickets')
    } catch (err: any) {
      setError(err.response?.data?.message || 'Booking failed')
    } finally {
      setBookingLoading(false)
    }
  }

  if (loading) return <LoadingSpinner fullScreen />
  if (!schedule) return <Container maxWidth="md" sx={{ py: 4 }}><Alert severity="error">Schedule not found</Alert></Container>

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Button startIcon={<ArrowBack />} onClick={() => navigate(-1)} sx={{ mb: 3 }}>
        Back to Search
      </Button>

      <Typography variant="h4" fontWeight={700} gutterBottom>
        Select Your Seat
      </Typography>

      <Grid container spacing={4}>
        {/* Seat Selection */}
        <Grid item xs={12} md={7}>
          <Paper sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
              <Typography variant="h6" fontWeight={600}>
                {schedule.route.origin} → {schedule.route.destination}
              </Typography>
              <Stack direction="row" spacing={1}>
                <Chip size="small" label="Available" sx={{ bgcolor: '#E5E7EB' }} />
                <Chip size="small" label="Locked" sx={{ bgcolor: '#F59E0B', color: 'white' }} />
                <Chip size="small" label="Booked" sx={{ bgcolor: '#EF4444', color: 'white' }} />
              </Stack>
            </Box>

            {/* Bus Layout */}
            <Box sx={{ overflowX: 'auto' }}>
              <Box sx={{ minWidth: 400, mx: 'auto' }}>
                {/* Driver */}
                <Box sx={{ mb: 3, p: 2, bgcolor: 'grey.100', borderRadius: 1, textAlign: 'center' }}>
                  <Typography variant="caption" color="text.secondary">Driver</Typography>
                </Box>

                {/* Seats Grid */}
                <Box className="seat-grid" sx={{ display: 'grid', gridTemplateColumns: `repeat(${schedule.bus.seatsPerRow}, 1fr)`, gap: 1, maxWidth: 400, mx: 'auto' }}>
                  {Array.from({ length: schedule.bus.capacity }, (_, i) => i + 1).map((seatNum) => {
                    const seat = seats.find(s => s.seatNumber === seatNum)
                    const isSelected = selectedSeat === seatNum
                    
                    return (
                      <Box
                        key={seatNum}
                        className={`seat ${seat?.status.toLowerCase() || 'available'} ${isSelected ? 'selected' : ''}`}
                        onClick={() => handleSeatSelect(seatNum)}
                        sx={{
                          bgcolor: isSelected ? 'primary.main' : 
                            seat?.status === 'Booked' ? 'error.main' :
                            seat?.status === 'Locked' ? 'warning.main' : 'grey.200',
                          color: seat?.status !== 'Available' ? 'white' : 'text.primary',
                          cursor: seat?.status === 'Available' ? 'pointer' : 'not-allowed',
                          opacity: seat?.status === 'Locked' ? 0.7 : 1,
                        }}
                      >
                        {seatNum}
                      </Box>
                    )
                  })}
                </Box>

                {/* Aisle indicator */}
                <Box sx={{ my: 2, textAlign: 'center' }}>
                  <Typography variant="caption" color="text.secondary">Aisle</Typography>
                </Box>
              </Box>
            </Box>

            <Box sx={{ mt: 3, p: 2, bgcolor: 'info.50', borderRadius: 1 }}>
              <Typography variant="body2" color="info.dark">
                <Warning fontSize="small" sx={{ verticalAlign: 'middle', mr: 1 }} />
                Seat is held for 5 minutes during booking. Please complete your booking quickly.
              </Typography>
            </Box>
          </Paper>
        </Grid>

        {/* Booking Details */}
        <Grid item xs={12} md={5}>
          <Card>
            <CardContent>
              <Typography variant="h6" fontWeight={600} gutterBottom>
                Trip Details
              </Typography>
              
              <Box sx={{ mb: 3 }}>
                <Typography variant="body2" color="text.secondary">Date</Typography>
                <Typography variant="body1">
                  {format(new Date(schedule.departureTime), 'EEEE, MMMM d, yyyy')}
                </Typography>
              </Box>

              <Box sx={{ mb: 3 }}>
                <Typography variant="body2" color="text.secondary">Departure</Typography>
                <Typography variant="h5" fontWeight={700}>
                  {format(new Date(schedule.departureTime), 'HH:mm')}
                </Typography>
              </Box>

              <Box sx={{ mb: 3 }}>
                <Typography variant="body2" color="text.secondary">Bus</Typography>
                <Typography variant="body1">
                  {schedule.bus.type} Bus • {schedule.bus.plateNumber}
                </Typography>
              </Box>

              <Divider sx={{ my: 2 }} />

              {selectedSeat ? (
                <>
                  <Box sx={{ mb: 3 }}>
                    <Typography variant="body2" color="text.secondary">Selected Seat</Typography>
                    <Typography variant="h4" fontWeight={700} color="primary.main">
                      {selectedSeat}
                    </Typography>
                  </Box>

                  <TextField
                    fullWidth
                    label="Passenger Name"
                    value={passengerName}
                    onChange={(e) => setPassengerName(e.target.value)}
                    required
                    InputProps={{
                      startAdornment: <Person color="action" sx={{ mr: 1 }} />,
                    }}
                    sx={{ mb: 2 }}
                  />

                  <TextField
                    fullWidth
                    label="Phone Number (optional)"
                    value={passengerPhone}
                    onChange={(e) => setPassengerPhone(e.target.value)}
                    InputProps={{
                      startAdornment: <Phone color="action" sx={{ mr: 1 }} />,
                    }}
                    sx={{ mb: 3 }}
                  />

                  {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
                  {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}

                  <Button
                    fullWidth
                    variant="contained"
                    size="large"
                    onClick={handleBooking}
                    disabled={bookingLoading || !passengerName}
                    startIcon={bookingLoading ? <CircularProgress size={20} color="inherit" /> : <CreditCard />}
                  >
                    {bookingLoading ? 'Processing...' : `Pay ${schedule.basePrice.toFixed(2)} TND`}
                  </Button>
                </>
              ) : (
                <Alert severity="info">
                  Please select a seat to continue
                </Alert>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  )
}
