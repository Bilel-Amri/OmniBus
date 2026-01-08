import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  Grid,
  TextField,
  InputAdornment,
  Chip,
  Avatar,
  Stack,
} from '@mui/material'
import {
  Search,
  DirectionsBus,
  LocationOn,
  Schedule,
  Security,
  AccessTime,
} from '@mui/icons-material'
import { DatePicker } from '@mui/x-date-pickers/DatePicker'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns'
import { format } from 'date-fns'

export default function HomePage() {
  const navigate = useNavigate()
  const [origin, setOrigin] = useState('')
  const [destination, setDestination] = useState('')
  const [date, setDate] = useState<Date | null>(new Date())

  const handleSearch = () => {
    const params = new URLSearchParams()
    if (origin) params.set('origin', origin)
    if (destination) params.set('destination', destination)
    if (date) params.set('date', format(date, 'yyyy-MM-dd'))
    navigate(`/search?${params.toString()}`)
  }

  const features = [
    {
      icon: <DirectionsBus sx={{ fontSize: 40 }} />,
      title: 'City & Intercity Routes',
      description: 'Travel anywhere in Tunisia with our extensive network of bus routes',
    },
    {
      icon: <Schedule sx={{ fontSize: 40 }} />,
      title: 'Real-Time Tracking',
      description: 'Track your bus in real-time and never miss a ride again',
    },
    {
      icon: <Security sx={{ fontSize: 40 }} />,
      title: 'Secure Booking',
      description: 'Book your tickets safely with our secure payment system',
    },
    {
      icon: <AccessTime sx={{ fontSize: 40 }} />,
      title: '24/7 Support',
      description: 'Our customer support team is always ready to help you',
    },
  ]

  const popularRoutes = [
    { origin: 'Tunis', destination: 'Sfax', price: 'From 25 TND', duration: '4h 30m' },
    { origin: 'Tunis', destination: 'Sousse', price: 'From 15 TND', duration: '2h 20m' },
    { origin: 'Tunis', destination: 'Bizerte', price: 'From 5 TND', duration: '1h 10m' },
    { origin: 'Sfax', destination: 'Gabes', price: 'From 12 TND', duration: '2h' },
  ]

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      {/* Hero Section */}
      <Box
        sx={{
          bgcolor: 'primary.main',
          color: 'white',
          py: { xs: 8, md: 12 },
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        <Container maxWidth="lg">
          <Grid container spacing={4} alignItems="center">
            <Grid item xs={12} md={7}>
              <Typography variant="h2" component="h1" fontWeight={700} gutterBottom>
                Your Journey Starts Here
              </Typography>
              <Typography variant="h6" sx={{ mb: 4, opacity: 0.9 }}>
                Book bus tickets for city and intercity travel across Tunisia with ease.
                Real-time tracking, secure payments, and instant confirmations.
              </Typography>
              
              {/* Search Box */}
              <Card sx={{ p: 3, boxShadow: 4 }}>
                <Grid container spacing={2} alignItems="center">
                  <Grid item xs={12} sm={4}>
                    <TextField
                      fullWidth
                      label="From"
                      value={origin}
                      onChange={(e) => setOrigin(e.target.value)}
                      InputProps={{
                        startAdornment: (
                          <InputAdornment position="start">
                            <LocationOn color="primary" />
                          </InputAdornment>
                        ),
                      }}
                    />
                  </Grid>
                  <Grid item xs={12} sm={4}>
                    <TextField
                      fullWidth
                      label="To"
                      value={destination}
                      onChange={(e) => setDestination(e.target.value)}
                      InputProps={{
                        startAdornment: (
                          <InputAdornment position="start">
                            <LocationOn color="primary" />
                          </InputAdornment>
                        ),
                      }}
                    />
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
                  <Grid item xs={12} sm={1}>
                    <Button
                      variant="contained"
                      size="large"
                      onClick={handleSearch}
                      sx={{ height: '100%', minHeight: 56 }}
                    >
                      <Search />
                    </Button>
                  </Grid>
                </Grid>
              </Card>
            </Grid>
            
            <Grid item xs={12} md={5} sx={{ display: { xs: 'none', md: 'block' } }}>
              <Box
                component="img"
                src="https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?w=600&h=400&fit=crop"
                alt="Bus"
                sx={{
                  width: '100%',
                  borderRadius: 4,
                  boxShadow: '0 20px 60px rgba(0,0,0,0.3)',
                }}
              />
            </Grid>
          </Grid>
        </Container>
      </Box>
      
      {/* Features Section */}
      <Container maxWidth="lg" sx={{ py: 8 }}>
        <Typography variant="h4" component="h2" fontWeight={700} textAlign="center" gutterBottom>
          Why Choose OmniBus?
        </Typography>
        <Typography variant="body1" color="text.secondary" textAlign="center" sx={{ mb: 6, maxWidth: 600, mx: 'auto' }}>
          Experience the best bus travel booking platform in Tunisia with features designed for your convenience
        </Typography>
        
        <Grid container spacing={4}>
          {features.map((feature, index) => (
            <Grid item xs={12} sm={6} md={3} key={index}>
              <Card sx={{ height: '100%', textAlign: 'center', p: 3 }}>
                <Avatar
                  sx={{
                    width: 80,
                    height: 80,
                    mx: 'auto',
                    mb: 2,
                    bgcolor: 'primary.50',
                    color: 'primary.main',
                  }}
                >
                  {feature.icon}
                </Avatar>
                <Typography variant="h6" fontWeight={600} gutterBottom>
                  {feature.title}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {feature.description}
                </Typography>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Container>
      
      {/* Popular Routes Section */}
      <Box sx={{ bgcolor: 'grey.50', py: 8 }}>
        <Container maxWidth="lg">
          <Typography variant="h4" component="h2" fontWeight={700} textAlign="center" gutterBottom>
            Popular Routes
          </Typography>
          <Typography variant="body1" color="text.secondary" textAlign="center" sx={{ mb: 6 }}>
            Explore our most booked routes across Tunisia
          </Typography>
          
          <Grid container spacing={3}>
            {popularRoutes.map((route, index) => (
              <Grid item xs={12} sm={6} md={3} key={index}>
                <Card sx={{ cursor: 'pointer', transition: 'transform 0.2s', '&:hover': { transform: 'translateY(-4px)' } }} onClick={() => {
                  setOrigin(route.origin)
                  setDestination(route.destination)
                  navigate('/search')
                }}>
                  <CardContent>
                    <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 2 }}>
                      <Chip label={route.origin} color="primary" size="small" />
                      <Typography color="text.secondary">→</Typography>
                      <Chip label={route.destination} color="secondary" size="small" />
                    </Stack>
                    <Typography variant="h6" color="primary" fontWeight={600}>
                      {route.price}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {route.duration}
                    </Typography>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
          
          <Box sx={{ textAlign: 'center', mt: 4 }}>
            <Button variant="outlined" size="large" onClick={() => navigate('/search')}>
              View All Routes
            </Button>
          </Box>
        </Container>
      </Box>
      
      {/* CTA Section */}
      <Container maxWidth="md" sx={{ py: 8, textAlign: 'center' }}>
        <Typography variant="h4" component="h2" fontWeight={700} gutterBottom>
          Ready to Travel?
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 4 }}>
          Join thousands of passengers who trust OmniBus for their travel needs
        </Typography>
        <Stack direction="row" spacing={2} justifyContent="center">
          <Button variant="contained" size="large" onClick={() => navigate('/search')}>
            Book Now
          </Button>
          <Button variant="outlined" size="large" onClick={() => navigate('/register')}>
            Create Account
          </Button>
        </Stack>
      </Container>
    </LocalizationProvider>
  )
}
