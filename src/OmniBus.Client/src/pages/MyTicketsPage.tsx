import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Paper,
  Card,
  CardContent,
  Grid,
  Chip,
  Button,
  CircularProgress,
  Alert,
  Tabs,
  Tab,
  Divider,
} from '@mui/material'
import { format } from 'date-fns'
import {
  ConfirmationNumber,
  QrCode,
  CalendarToday,
  LocationOn,
  Schedule,
  ArrowForward,
} from '@mui/icons-material'
import { ticketService } from '../services/api'

interface Ticket {
  id: string
  bookingReference: string
  seatNumber: number
  status: string
  price: number
  bookingDate: string
  qrCode?: string
  departureTime: string
  origin: string
  destination: string
  busType: string
}

export default function MyTicketsPage() {
  const navigate = useNavigate()
  const [tab, setTab] = useState(0)
  const [upcoming, setUpcoming] = useState<Ticket[]>([])
  const [history, setHistory] = useState<Ticket[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchTickets()
  }, [])

  const fetchTickets = async () => {
    try {
      const [upcomingRes, historyRes] = await Promise.all([
        ticketService.getUpcoming(),
        ticketService.getHistory(),
      ])
      setUpcoming(upcomingRes.data)
      setHistory(historyRes.data)
    } catch (err: any) {
      setError('Failed to load tickets')
    } finally {
      setLoading(false)
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Booked': return 'primary'
      case 'Completed': return 'success'
      case 'Cancelled': return 'error'
      case 'Reserved': return 'warning'
      default: return 'default'
    }
  }

  const tickets = tab === 0 ? upcoming : history

  if (loading) {
    return (
      <Container maxWidth="md" sx={{ py: 8, textAlign: 'center' }}>
        <CircularProgress />
      </Container>
    )
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" fontWeight={700} gutterBottom>
        My Tickets
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label={`Upcoming (${upcoming.length})`} />
        <Tab label={`History (${history.length})`} />
      </Tabs>

      {tickets.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <ConfirmationNumber sx={{ fontSize: 64, color: 'grey.300', mb: 2 }} />
          <Typography variant="h6" color="text.secondary" gutterBottom>
            No {tab === 0 ? 'upcoming' : 'past'} tickets
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            {tab === 0 
              ? "You don't have any upcoming trips. Start planning your journey!" 
              : "Your completed trips will appear here."}
          </Typography>
          <Button variant="contained" onClick={() => navigate('/search')}>
            Search Routes
          </Button>
        </Paper>
      ) : (
        <Stack spacing={3}>
          {tickets.map((ticket) => (
            <Card key={ticket.id} sx={{ transition: 'box-shadow 0.2s', '&:hover': { boxShadow: 4 } }}>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                  <Box>
                    <Typography variant="h6" fontWeight={600}>
                      {ticket.origin} → {ticket.destination}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Booking Ref: {ticket.bookingReference}
                    </Typography>
                  </Box>
                  <Chip 
                    label={ticket.status} 
                    color={getStatusColor(ticket.status) as any} 
                    size="small" 
                  />
                </Box>

                <Grid container spacing={2}>
                  <Grid item xs={6} sm={3}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <CalendarToday color="action" fontSize="small" />
                      <Box>
                        <Typography variant="caption" color="text.secondary">Date</Typography>
                        <Typography variant="body2">
                          {format(new Date(ticket.departureTime), 'EEE, MMM d')}
                        </Typography>
                      </Box>
                    </Box>
                  </Grid>
                  <Grid item xs={6} sm={3}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Schedule color="action" fontSize="small" />
                      <Box>
                        <Typography variant="caption" color="text.secondary">Departure</Typography>
                        <Typography variant="body2">
                          {format(new Date(ticket.departureTime), 'HH:mm')}
                        </Typography>
                      </Box>
                    </Box>
                  </Grid>
                  <Grid item xs={6} sm={3}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <LocationOn color="action" fontSize="small" />
                      <Box>
                        <Typography variant="caption" color="text.secondary">Seat</Typography>
                        <Typography variant="body2">#{ticket.seatNumber}</Typography>
                      </Box>
                    </Box>
                  </Grid>
                  <Grid item xs={6} sm={3}>
                    <Typography variant="caption" color="text.secondary">Price</Typography>
                    <Typography variant="h6" color="primary.main" fontWeight={700}>
                      {ticket.price.toFixed(2)} TND
                    </Typography>
                  </Grid>
                </Grid>

                <Divider sx={{ my: 2 }} />

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  {ticket.qrCode && (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <QrCode color="action" />
                      <Typography variant="body2">QR Code Available</Typography>
                    </Box>
                  )}
                  
                  <Box sx={{ display: 'flex', gap: 1 }}>
                    {ticket.status === 'Booked' && (
                      <Button size="small" variant="outlined" color="error">
                        Cancel
                      </Button>
                    )}
                    <Button 
                      size="small" 
                      variant="contained" 
                      endIcon={<ArrowForward />}
                      onClick={() => navigate(`/tickets/${ticket.id}`)}
                    >
                      View Details
                    </Button>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          ))}
        </Stack>
      )}
    </Container>
  )
}

import { Stack } from '@mui/material'
