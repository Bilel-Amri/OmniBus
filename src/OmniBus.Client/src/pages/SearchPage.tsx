import { useState, useEffect } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  TextField,
  Button,
  Card,
  CardContent,
  Chip,
  Stack,
  CircularProgress,
  Alert,
  InputAdornment,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
} from '@mui/material'
import { DatePicker } from '@mui/x-date-pickers/DatePicker'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import { format } from 'date-fns'
import {
  Search,
  LocationOn,
  Schedule,
  AttachMoney,
  DirectionsBus,
} from '@mui/icons-material'
import { scheduleService, routeService } from '../services/api'

interface Schedule {
  id: string
  departureTime: string
  arrivalTime: string
  availableSeats: number
  basePrice: number
  bus: {
    plateNumber: string
    type: string
    capacity: number
    hasAirConditioning: boolean
    hasWifi: boolean
  }
  route: {
    origin: string
    destination: string
    distanceKm: number
  }
  driverName?: string
}

export default function SearchPage() {
  const navigate = useNavigate()
  const [searchParams, setSearchParams] = useSearchParams()
  
  const [origin, setOrigin] = useState(searchParams.get('origin') || '')
  const [destination, setDestination] = useState(searchParams.get('destination') || '')
  const [date, setDate] = useState<Date | null>(
    searchParams.get('date') ? new Date(searchParams.get('date')!) : new Date()
  )
  const [schedules, setSchedules] = useState<Schedule[]>([])
  const [cities, setCities] = useState<string[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  // Load cities on mount
  useEffect(() => {
    const loadCities = async () => {
      try {
        const response = await routeService.getCities()
        setCities(response.data)
      } catch (err) {
        console.error('Failed to load cities:', err)
      }
    }
    loadCities()
  }, [])

  const handleSearch = async () => {
    setLoading(true)
    setError('')
    
    try {
      const response = await scheduleService.search(
        origin,
        destination,
        date ? format(date, 'yyyy-MM-dd') : ''
      )
      setSchedules(response.data)
      
      // Update URL
      const params = new URLSearchParams()
      if (origin) params.set('origin', origin)
      if (destination) params.set('destination', destination)
      if (date) params.set('date', format(date, 'yyyy-MM-dd'))
      setSearchParams(params)
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to search schedules')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (searchParams.get('origin') || searchParams.get('destination') || searchParams.get('date')) {
      handleSearch()
    }
  }, [])

  const getDuration = (departure: string, arrival: string) => {
    const dep = new Date(departure)
    const arr = new Date(arrival)
    const diffMs = arr.getTime() - dep.getTime()
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
    const diffMinutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60))
    return `${diffHours}h ${diffMinutes}m`
  }

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Typography variant="h4" fontWeight={700} gutterBottom>
          Search Routes
        </Typography>
        
        {/* Search Form */}
        <Paper sx={{ p: 3, mb: 4 }} elevation={2}>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} sm={3}>
              <FormControl fullWidth>
                <InputLabel>From</InputLabel>
                <Select
                  value={origin}
                  onChange={(e) => setOrigin(e.target.value)}
                  label="From"
                >
                  <MenuItem value="">
                    <em>Select city</em>
                  </MenuItem>
                  {cities.map((city) => (
                    <MenuItem key={city} value={city}>
                      {city}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} sm={3}>
              <FormControl fullWidth>
                <InputLabel>To</InputLabel>
                <Select
                  value={destination}
                  onChange={(e) => setDestination(e.target.value)}
                  label="To"
                >
                  <MenuItem value="">
                    <em>Select city</em>
                  </MenuItem>
                  {cities.map((city) => (
                    <MenuItem key={city} value={city}>
                      {city}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} sm={3}>
              <DatePicker
                label="Date"
                value={date}
                onChange={(newValue) => setDate(newValue)}
                slotProps={{
                  textField: { fullWidth: true },
                }}
              />
            </Grid>
            <Grid item xs={12} sm={3}>
              <Button
                fullWidth
                variant="contained"
                size="large"
                onClick={handleSearch}
                disabled={loading}
                startIcon={loading ? <CircularProgress size={20} color="inherit" /> : <Search />}
                sx={{ height: 56 }}
              >
                Search
              </Button>
            </Grid>
          </Grid>
        </Paper>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {/* Results */}
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
            <CircularProgress />
          </Box>
        ) : schedules.length > 0 ? (
          <Stack spacing={2}>
            <Typography variant="body2" color="text.secondary">
              {schedules.length} buses found
            </Typography>
            
            {schedules.map((schedule) => (
              <Card key={schedule.id} sx={{ transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 4 } }}>
                <CardContent>
                  <Grid container spacing={2} alignItems="center">
                    <Grid item xs={12} md={3}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <DirectionsBus color="primary" />
                        <Box>
                          <Typography variant="subtitle1" fontWeight={600}>
                            {schedule.route.origin} → {schedule.route.destination}
                          </Typography>
                          <Typography variant="body2" color="text.secondary">
                            {schedule.bus.type} Bus
                          </Typography>
                        </Box>
                      </Box>
                    </Grid>
                    
                    <Grid item xs={12} md={3}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Schedule color="action" />
                        <Box>
                          <Typography variant="h6" fontWeight={600}>
                            {format(new Date(schedule.departureTime), 'HH:mm')}
                          </Typography>
                          <Typography variant="body2" color="text.secondary">
                            {format(new Date(schedule.departureTime), 'EEE, MMM d')}
                          </Typography>
                        </Box>
                      </Box>
                      <Box sx={{ ml: 4, mt: 0.5 }}>
                        <Typography variant="body2" color="text.secondary">
                          Arrival: {format(new Date(schedule.arrivalTime), 'HH:mm')}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          Duration: {getDuration(schedule.departureTime, schedule.arrivalTime)}
                        </Typography>
                      </Box>
                    </Grid>
                    
                    <Grid item xs={6} md={2}>
                      <Stack direction="row" spacing={1}>
                        {schedule.bus.hasAirConditioning && (
                          <Chip label="A/C" size="small" color="info" />
                        )}
                        {schedule.bus.hasWifi && (
                          <Chip label="WiFi" size="small" color="success" />
                        )}
                      </Stack>
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                        {schedule.availableSeats} seats available
                      </Typography>
                    </Grid>
                    
                    <Grid item xs={6} md={2}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <AttachMoney color="primary" />
                        <Typography variant="h5" fontWeight={700} color="primary.main">
                          {schedule.basePrice.toFixed(2)}
                        </Typography>
                      </Box>
                      <Typography variant="caption" color="text.secondary">
                        TND
                      </Typography>
                    </Grid>
                    
                    <Grid item xs={12} md={2}>
                      <Button
                        fullWidth
                        variant="contained"
                        size="large"
                        onClick={() => navigate(`/booking/${schedule.id}`)}
                        disabled={schedule.availableSeats === 0}
                      >
                        {schedule.availableSeats === 0 ? 'Sold Out' : 'Select Seat'}
                      </Button>
                    </Grid>
                  </Grid>
                </CardContent>
              </Card>
            ))}
          </Stack>
        ) : (
          searchParams.get('origin') && (
            <Box sx={{ textAlign: 'center', py: 8 }}>
              <DirectionsBus sx={{ fontSize: 64, color: 'grey.300', mb: 2 }} />
              <Typography variant="h6" color="text.secondary">
                No buses found for your search
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Try different dates or routes
              </Typography>
            </Box>
          )
        )}
      </Container>
    </LocalizationProvider>
  )
}
