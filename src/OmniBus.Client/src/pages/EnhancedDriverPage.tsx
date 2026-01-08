import { useState, useEffect } from 'react';
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
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Badge,
  Avatar,
} from '@mui/material';
import { format } from 'date-fns';
import {
  LocationOn,
  Schedule,
  Warning,
  PlayArrow,
  StopCircle,
  AccessTime,
  QrCodeScanner,
  People,
  CheckCircle,
  Cancel,
  Refresh,
} from '@mui/icons-material';
import { driverService } from '../services/api';
import { signalRService } from '../services/signalR';
import LoadingSpinner from '../components/common/LoadingSpinner';

interface Passenger {
  id: string;
  name: string;
  email: string;
  seatNumber: number;
  ticketCode: string;
  boardingStatus: 'pending' | 'boarded' | 'no-show';
}

interface Trip {
  id: string;
  departureTime: string;
  arrivalTime: string;
  status: string;
  availableSeats: number;
  route: {
    origin: string;
    destination: string;
    distanceKm: number;
  };
  bus: {
    plateNumber: string;
    type: string;
    capacity: number;
  };
  passengers?: Passenger[];
  currentLocation?: {
    latitude: number;
    longitude: number;
    timestamp: Date;
  };
}

export default function EnhancedDriverPage() {
  const [trips, setTrips] = useState<Trip[]>([]);
  const [todayTrips, setTodayTrips] = useState<Trip[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedTrip, setSelectedTrip] = useState<Trip | null>(null);
  const [delayDialogOpen, setDelayDialogOpen] = useState(false);
  const [delayMinutes, setDelayMinutes] = useState(0);
  const [delayReason, setDelayReason] = useState('');
  const [locationDialogOpen, setLocationDialogOpen] = useState(false);
  const [passengersDialogOpen, setPassengersDialogOpen] = useState(false);
  const [qrScannerOpen, setQrScannerOpen] = useState(false);
  const [scannedCode, setScannedCode] = useState('');

  useEffect(() => {
    fetchTrips();

    // Connect to SignalR for real-time updates
    signalRService.connectToTrackingHub();

    return () => {
      signalRService.disconnect();
    };
  }, []);

  const fetchTrips = async () => {
    try {
      const [allTripsRes, todayRes] = await Promise.all([
        driverService.getTrips(),
        driverService.getToday(),
      ]);
      setTrips(allTripsRes.data);
      setTodayTrips(todayRes.data);
    } catch (err: any) {
      setError('Failed to load trips');
    } finally {
      setLoading(false);
    }
  };

  const handleStartTrip = async (tripId: string) => {
    try {
      await driverService.startTrip(tripId);
      fetchTrips();
    } catch (err: any) {
      setError('Failed to start trip');
    }
  };

  const handleCompleteTrip = async (tripId: string) => {
    try {
      await driverService.completeTrip(tripId);
      fetchTrips();
    } catch (err: any) {
      setError('Failed to complete trip');
    }
  };

  const handleReportDelay = async () => {
    if (!selectedTrip) return;
    try {
      await driverService.reportDelay(selectedTrip.id, delayMinutes, delayReason);
      setDelayDialogOpen(false);
      setDelayMinutes(0);
      setDelayReason('');
      fetchTrips();
    } catch (err: any) {
      setError('Failed to report delay');
    }
  };

  const handleBoardPassenger = async (tripId: string, ticketCode: string) => {
    try {
      // Call API to mark passenger as boarded
      // await driverService.boardPassenger(tripId, ticketCode);
      if (selectedTrip) {
        const updatedPassengers = selectedTrip.passengers?.map((p) =>
          p.ticketCode === ticketCode ? { ...p, boardingStatus: 'boarded' as const } : p
        );
        setSelectedTrip({ ...selectedTrip, passengers: updatedPassengers });
      }
      setScannedCode('');
    } catch (err: any) {
      setError('Failed to board passenger');
    }
  };

  const handleQrScan = () => {
    // In production, this would use a real QR scanner library
    if (scannedCode && selectedTrip) {
      handleBoardPassenger(selectedTrip.id, scannedCode);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Scheduled':
        return 'info';
      case 'InProgress':
        return 'primary';
      case 'Completed':
        return 'success';
      case 'Delayed':
        return 'warning';
      case 'Cancelled':
        return 'error';
      default:
        return 'default';
    }
  };

  const getBoardingStats = (passengers?: Passenger[]) => {
    if (!passengers) return { boarded: 0, pending: 0, noShow: 0 };
    return {
      boarded: passengers.filter((p) => p.boardingStatus === 'boarded').length,
      pending: passengers.filter((p) => p.boardingStatus === 'pending').length,
      noShow: passengers.filter((p) => p.boardingStatus === 'no-show').length,
    };
  };

  if (loading) return <LoadingSpinner fullScreen />;

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Box display="flex" alignItems="center" justifyContent="space-between" mb={3}>
        <Typography variant="h4" fontWeight={700}>
          🚌 Driver Dashboard
        </Typography>
        <Button startIcon={<Refresh />} onClick={fetchTrips} variant="outlined">
          Refresh
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      {/* Today's Trips */}
      <Paper sx={{ p: 3, mb: 4 }}>
        <Typography variant="h6" fontWeight={600} gutterBottom>
          📅 Today's Trips
        </Typography>

        {todayTrips.length === 0 ? (
          <Box sx={{ textAlign: 'center', py: 4 }}>
            <Schedule sx={{ fontSize: 48, color: 'grey.300', mb: 2 }} />
            <Typography color="text.secondary">No trips scheduled for today</Typography>
          </Box>
        ) : (
          <Grid container spacing={3}>
            {todayTrips.map((trip) => {
              const stats = getBoardingStats(trip.passengers);
              return (
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
                          <Typography variant="body2" color="text.secondary">
                            Departure
                          </Typography>
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
                          {trip.passengers && (
                            <Box mt={1}>
                              <Typography variant="caption" color="text.secondary">
                                {stats.boarded}/{trip.passengers.length} boarded
                              </Typography>
                            </Box>
                          )}
                        </Grid>

                        <Grid item xs={12} md={5}>
                          <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', justifyContent: 'flex-end' }}>
                            {trip.status === 'Scheduled' && (
                              <>
                                <Button
                                  variant="outlined"
                                  size="small"
                                  startIcon={<People />}
                                  onClick={() => {
                                    setSelectedTrip(trip);
                                    setPassengersDialogOpen(true);
                                  }}
                                >
                                  Passengers
                                </Button>
                                <Button
                                  variant="outlined"
                                  color="warning"
                                  size="small"
                                  startIcon={<AccessTime />}
                                  onClick={() => {
                                    setSelectedTrip(trip);
                                    setDelayDialogOpen(true);
                                  }}
                                >
                                  Report Delay
                                </Button>
                                <Button
                                  variant="contained"
                                  size="small"
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
                                  size="small"
                                  startIcon={<QrCodeScanner />}
                                  onClick={() => {
                                    setSelectedTrip(trip);
                                    setQrScannerOpen(true);
                                  }}
                                >
                                  Scan QR
                                </Button>
                                <Button
                                  variant="outlined"
                                  size="small"
                                  startIcon={<People />}
                                  onClick={() => {
                                    setSelectedTrip(trip);
                                    setPassengersDialogOpen(true);
                                  }}
                                >
                                  Passengers
                                </Button>
                                <Button
                                  variant="outlined"
                                  size="small"
                                  startIcon={<LocationOn />}
                                  onClick={() => setLocationDialogOpen(true)}
                                >
                                  Update Location
                                </Button>
                                <Button
                                  variant="contained"
                                  color="success"
                                  size="small"
                                  startIcon={<StopCircle />}
                                  onClick={() => handleCompleteTrip(trip.id)}
                                >
                                  Complete
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
              );
            })}
          </Grid>
        )}
      </Paper>

      {/* All Upcoming Trips */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" fontWeight={600} gutterBottom>
          📆 Upcoming Trips
        </Typography>

        <List>
          {trips
            .filter((t) => t.status === 'Scheduled' || t.status === 'Delayed')
            .slice(0, 5)
            .map((trip, index) => (
              <Box key={trip.id}>
                <ListItem>
                  <ListItemText
                    primary={`${trip.route.origin} → ${trip.route.destination}`}
                    secondary={`${format(new Date(trip.departureTime), 'EEE, MMM d')} at ${format(
                      new Date(trip.departureTime),
                      'HH:mm'
                    )}`}
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

      {/* Passengers Dialog */}
      <Dialog
        open={passengersDialogOpen}
        onClose={() => setPassengersDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          Passenger List - {selectedTrip?.route.origin} → {selectedTrip?.route.destination}
        </DialogTitle>
        <DialogContent>
          {selectedTrip?.passengers && selectedTrip.passengers.length > 0 ? (
            <>
              <Box mb={2}>
                <Grid container spacing={2}>
                  <Grid item xs={4}>
                    <Paper sx={{ p: 2, bgcolor: 'success.light', color: 'white' }}>
                      <Typography variant="h4">{getBoardingStats(selectedTrip.passengers).boarded}</Typography>
                      <Typography variant="body2">Boarded</Typography>
                    </Paper>
                  </Grid>
                  <Grid item xs={4}>
                    <Paper sx={{ p: 2, bgcolor: 'warning.light', color: 'white' }}>
                      <Typography variant="h4">{getBoardingStats(selectedTrip.passengers).pending}</Typography>
                      <Typography variant="body2">Pending</Typography>
                    </Paper>
                  </Grid>
                  <Grid item xs={4}>
                    <Paper sx={{ p: 2, bgcolor: 'error.light', color: 'white' }}>
                      <Typography variant="h4">{getBoardingStats(selectedTrip.passengers).noShow}</Typography>
                      <Typography variant="body2">No-Show</Typography>
                    </Paper>
                  </Grid>
                </Grid>
              </Box>
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Seat</TableCell>
                      <TableCell>Passenger</TableCell>
                      <TableCell>Ticket Code</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {selectedTrip.passengers
                      .sort((a, b) => a.seatNumber - b.seatNumber)
                      .map((passenger) => (
                        <TableRow key={passenger.id}>
                          <TableCell>
                            <Badge badgeContent={passenger.seatNumber} color="primary">
                              <Avatar sx={{ width: 24, height: 24 }}></Avatar>
                            </Badge>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontWeight={600}>
                              {passenger.name}
                            </Typography>
                            <Typography variant="caption" color="text.secondary">
                              {passenger.email}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Typography variant="body2" fontFamily="monospace">
                              {passenger.ticketCode}
                            </Typography>
                          </TableCell>
                          <TableCell>
                            <Chip
                              label={passenger.boardingStatus}
                              color={
                                passenger.boardingStatus === 'boarded'
                                  ? 'success'
                                  : passenger.boardingStatus === 'pending'
                                  ? 'warning'
                                  : 'error'
                              }
                              size="small"
                            />
                          </TableCell>
                          <TableCell>
                            {passenger.boardingStatus === 'pending' && (
                              <IconButton
                                size="small"
                                color="success"
                                onClick={() =>
                                  handleBoardPassenger(selectedTrip.id, passenger.ticketCode)
                                }
                              >
                                <CheckCircle />
                              </IconButton>
                            )}
                          </TableCell>
                        </TableRow>
                      ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </>
          ) : (
            <Alert severity="info">No passengers have booked this trip yet.</Alert>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPassengersDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* QR Scanner Dialog */}
      <Dialog open={qrScannerOpen} onClose={() => setQrScannerOpen(false)}>
        <DialogTitle>
          <QrCodeScanner /> Scan Passenger QR Code
        </DialogTitle>
        <DialogContent>
          <Alert severity="info" sx={{ mb: 2 }}>
            In production, this would use the device camera to scan QR codes automatically.
          </Alert>
          <TextField
            fullWidth
            label="Enter Ticket Code"
            value={scannedCode}
            onChange={(e) => setScannedCode(e.target.value)}
            placeholder="e.g., TKT-12345"
            autoFocus
            onKeyPress={(e) => {
              if (e.key === 'Enter') {
                handleQrScan();
              }
            }}
          />
          <Box mt={3} p={3} bgcolor="grey.100" borderRadius={2} textAlign="center">
            <QrCodeScanner sx={{ fontSize: 80, color: 'grey.400' }} />
            <Typography variant="body2" color="text.secondary" mt={1}>
              Camera preview would appear here
            </Typography>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setQrScannerOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleQrScan} disabled={!scannedCode}>
            Confirm Boarding
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delay Dialog */}
      <Dialog open={delayDialogOpen} onClose={() => setDelayDialogOpen(false)}>
        <DialogTitle>Report Delay</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            type="number"
            label="Delay (minutes)"
            value={delayMinutes}
            onChange={(e) => setDelayMinutes(parseInt(e.target.value) || 0)}
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
                const { latitude, longitude } = position.coords;
                // Would call driverService.updateLocation({ latitude, longitude })
                console.log('Location:', latitude, longitude);
                setLocationDialogOpen(false);
              });
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
  );
}
