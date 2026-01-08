import { useState, useEffect } from 'react'
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  Button,
  Alert,
  Chip,
  List,
  ListItem,
  ListItemText,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
} from '@mui/material'
import { format } from 'date-fns'
import {
  LocationOn,
  Schedule,
  Warning,
  PlayArrow,
  StopCircle,
  AccessTime,
} from '@mui/icons-material'
import { driverService } from '../services/api'
import { signalRService } from '../services/signalR'
import LoadingSpinner from '../components/common/LoadingSpinner'
import { useAuth } from '../context/AuthContext'

interface Trip {
  id: string
  departureTime: string
  arrivalTime: string
  status: string
  availableSeats: number
  route: {
    origin: string
    destination: string
    distanceKm: number
  }
  bus: {
    plateNumber: string
    type: string
  }
  currentLocation?: {
    latitude: number
    longitude: number
    timestamp: Date
  }
}

export default function DriverPage() {
  const { isDriver, user } = useAuth()
  const [trips, setTrips] = useState<Trip[]>([])
  const [todayTrips, setTodayTrips] = useState<Trip[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [selectedTrip, setSelectedTrip] = useState<Trip | null>(null)
  const [delayDialogOpen, setDelayDialogOpen] = useState(false)
  const [delayMinutes, setDelayMinutes] = useState(0)
  const [delayReason, setDelayReason] = useState('')
  const [locationDialogOpen, setLocationDialogOpen] = useState(false)

  useEffect(() => {
    // Guard: only drivers should call driver endpoints
    if (!isDriver) {
      setError('Driver account required to view trips.')
      setLoading(false)
      return
    }

    fetchTrips()
    
    // Connect to SignalR for real-time updates
    signalRService.connectToTrackingHub()
    
    return () => {
      signalRService.disconnect()
    }
  }, [])

  const fetchTrips = async () => {
    setLoading(true)
    setError('')

    // Fetch upcoming/all trips
    try {
      const allTripsRes = await driverService.getTrips()
      setTrips(allTripsRes.data)
    } catch (err: any) {
      const status = err?.response?.status
      const msg = err?.response?.data?.message || err?.response?.data?.error || err?.message
      // Preserve detailed reason to help debugging
      setError(status === 403
        ? 'Access denied: Driver role required.'
        : status === 401
          ? 'Session expired. Please log in again.'
          : `Failed to load trips${msg ? `: ${msg}` : ''}`)
    }

    // Fetch today's trips (do not block UI if this fails)
    try {
      const todayRes = await driverService.getToday()
      setTodayTrips(todayRes.data)
    } catch (err: any) {
      // Only set a non-blocking message in console; keep UI usable
      const status = err?.response?.status
      const msg = err?.response?.data?.message || err?.response?.data?.error || err?.message
      console.warn('Failed to load today\'s trips', { status, msg })
    } finally {
      setLoading(false)
    }
  }

  const handleStartTrip = async (tripId: string) => {
    try {
      await driverService.startTrip(tripId)
      fetchTrips()
    } catch (err: any) {
      setError('Failed to start trip')
    }
  }

  const handleCompleteTrip = async (tripId: string) => {
    try {
      await driverService.completeTrip(tripId)
      fetchTrips()
    } catch (err: any) {
      setError('Failed to complete trip')
    }
  }

  const handleReportDelay = async () => {
    if (!selectedTrip) return
    try {
      await driverService.reportDelay(selectedTrip.id, delayMinutes, delayReason)
      setDelayDialogOpen(false)
      setDelayMinutes(0)
      setDelayReason('')
      fetchTrips()
    } catch (err: any) {
      setError('Failed to report delay')
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Scheduled': return 'info'
      case 'InProgress': return 'primary'
      case 'Completed': return 'success'
      case 'Delayed': return 'warning'
      case 'Cancelled': return 'error'
      default: return 'default'
    }
  }

  if (loading) return <LoadingSpinner fullScreen />

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" fontWeight={700} gutterBottom>
        Driver Dashboard
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      {!isDriver && (
        <Alert severity="info" sx={{ mb: 3 }}>
          You are logged in as {user?.role}. Switch to the driver account (driver@omnibus.tn) to access this dashboard.
        </Alert>
      )}

      {/* Today's Trips */}
      <Paper sx={{ p: 3, mb: 4 }}>
        <Typography variant="h6" fontWeight={600} gutterBottom>
          Today's Trips
        </Typography>
        
        {todayTrips.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 4 }}>
            <Schedule sx={{ fontSize: 48, color: 'grey.300', mb: 2 }} />
            <Typography color="text.secondary">
              No trips scheduled for today
            </Typography>
          </Box>
        ) : (
          <Grid container spacing={3}>
            {todayTrips.map((trip) => (
              <Grid item xs={12} key={trip.id}>
                <Card>
                  <CardContent>
                    <Grid container spacing={2} alignItems="center">
                      <Grid item xs={12} md={3}>
                        <Box>
                          <Typography variant="h6" fontWeight={600}>
                            {trip.route.origin} → {trip.route.destination}
                          </Typography>
                          <Typography variant="body2" color="text.secondary">
                            {trip.bus.plateNumber} • {trip.bus.type}
                          </Typography>
                        </Box>
                      </Grid>
                      
                      <Grid item xs={6} md={2}>
                        <Typography variant="body2" color="text.secondary">Departure</Typography>
                        <Typography variant="h6">
                          {format(new Date(trip.departureTime), 'HH:mm')}
                        </Typography>
                      </Grid>
                      
                      <Grid item xs={6} md={2}>
                        <Chip 
                          label={trip.status} 
                          color={getStatusColor(trip.status) as any} 
                          size="small" 
                        />
                      </Grid>
                      
                      <Grid item xs={12} md={5}>
                        <Box sx={{ display: 'flex', gap: 1, justifyContent: 'flex-end' }}>
                          {trip.status === 'Scheduled' && (
                            <>
                              <Button
                                variant="outlined"
                                color="warning"
                                startIcon={<AccessTime />}
                                onClick={() => {
                                  setSelectedTrip(trip)
                                  setDelayDialogOpen(true)
                                }}
                              >
                                Report Delay
                              </Button>
                              <Button
                                variant="contained"
                                startIcon={<PlayArrow />}
                                onClick={() => handleStartTrip(trip.id)}
                              >
                                Start Trip
                              </Button>
                            </>
                          )}
                          {trip.status === 'InProgress' && (
                            <>
                              <Button
                                variant="outlined"
                                startIcon={<LocationOn />}
                                onClick={() => setLocationDialogOpen(true)}
                              >
                                Update Location
                              </Button>
                              <Button
                                variant="contained"
                                color="success"
                                startIcon={<StopCircle />}
                                onClick={() => handleCompleteTrip(trip.id)}
                              >
                                Complete Trip
                              </Button>
                            </>
                          )}
                          {trip.status === 'Delayed' && (
                            <Alert severity="warning" icon={<Warning />}>
                              Delayed - Check details
                            </Alert>
                          )}
                        </Box>
                      </Grid>
                    </Grid>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        )}
      </Paper>

      {/* All Upcoming Trips */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" fontWeight={600} gutterBottom>
          Upcoming Trips
        </Typography>
        
        <List>
          {trips
            .filter(t => t.status === 'Scheduled' || t.status === 'Delayed')
            .slice(0, 5)
            .map((trip, index) => (
              <Box key={trip.id}>
                <ListItem>
                  <ListItemText
                    primary={`${trip.route.origin} → ${trip.route.destination}`}
                    secondary={`${format(new Date(trip.departureTime), 'EEE, MMM d')} at ${format(new Date(trip.departureTime), 'HH:mm')}`}
                  />
                  <Chip 
                    label={trip.status} 
                    color={getStatusColor(trip.status) as any} 
                    size="small" 
                  />
                </ListItem>
                {index < 4 && <Divider />}
              </Box>
            ))}
        </List>
      </Paper>

      {/* Delay Dialog */}
      <Dialog open={delayDialogOpen} onClose={() => setDelayDialogOpen(false)}>
        <DialogTitle>Report Delay</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            type="number"
            label="Delay (minutes)"
            value={delayMinutes}
            onChange={(e) => setDelayMinutes(parseInt(e.target.value))}
            sx={{ mt: 2, mb: 2 }}
          />
          <TextField
            fullWidth
            label="Reason (optional)"
            value={delayReason}
            onChange={(e) => setDelayReason(e.target.value)}
            multiline
            rows={2}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDelayDialogOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleReportDelay}>
            Report Delay
          </Button>
        </DialogActions>
      </Dialog>

      {/* Location Update Dialog */}
      <Dialog open={locationDialogOpen} onClose={() => setLocationDialogOpen(false)}>
        <DialogTitle>Update Location</DialogTitle>
        <DialogContent>
          <Alert severity="info" sx={{ mt: 2 }}>
            In a production app, this would use GPS coordinates from the device.
          </Alert>
          <Button
            fullWidth
            variant="contained"
            sx={{ mt: 3 }}
            onClick={() => {
              // Simulate location update
              navigator.geolocation.getCurrentPosition((position) => {
                const { latitude, longitude } = position.coords
                // Would call driverService.updateLocation({ latitude, longitude })
                console.log('Location:', latitude, longitude)
                setLocationDialogOpen(false)
              })
            }}
          >
            Get Current Location
          </Button>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setLocationDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </Container>
  )
}
