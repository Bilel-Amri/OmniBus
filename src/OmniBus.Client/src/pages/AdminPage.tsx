import { useState, useEffect } from 'react'
import { Routes, Route, Link, useLocation } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Paper,
  Tabs,
  Tab,
  Card,
  CardContent,
  Grid,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Checkbox,
  FormControlLabel,
} from '@mui/material'
import {
  Add,
  Edit,
  DirectionsBus,
  Route as RouteIcon,
  Schedule as ScheduleIcon,
  People,
} from '@mui/icons-material'
import { format } from 'date-fns'
import { busService, routeService, scheduleService } from '../services/api'
import LoadingSpinner from '../components/common/LoadingSpinner'

interface Bus {
  id: string
  plateNumber: string
  capacity: number
  availableSeats: number
  type: string
  status: string
}

interface Route {
  id: string
  name: string
  origin: string
  destination: string
  distanceKm: number
  isActive: boolean
}

interface Schedule {
  id: string
  busId: string
  routeId: string
  departureTime: string
  arrivalTime: string
  status: string
  basePrice: number
  availableSeats: number
  isRecurring: boolean
  operatingDays: number[]
  route: { origin: string; destination: string }
  bus: { plateNumber: string }
}

function BusManagement() {
  const [buses, setBuses] = useState<Bus[]>([])
  const [loading, setLoading] = useState(true)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editBus, setEditBus] = useState<Bus | null>(null)
  const [formData, setFormData] = useState({
    plateNumber: '',
    registrationNumber: '',
    type: 1, // Intercity
    capacity: 45,
    brand: '',
    model: '',
    seatsPerRow: 4,
    hasAirConditioning: true,
    hasWifi: false,
    isWheelchairAccessible: false,
  })

  useEffect(() => {
    fetchBuses()
  }, [])

  useEffect(() => {
    if (editBus) {
      const typeMap: Record<string, number> = { City: 0, Intercity: 1, Express: 2 }
      setFormData({
        plateNumber: editBus.plateNumber || '',
        registrationNumber: '',
        type: typeMap[editBus.type] || 1,
        capacity: editBus.capacity || 45,
        brand: '',
        model: '',
        seatsPerRow: 4,
        hasAirConditioning: true,
        hasWifi: false,
        isWheelchairAccessible: false,
      })
    } else {
      setFormData({
        plateNumber: '',
        registrationNumber: '',
        type: 1,
        capacity: 45,
        brand: '',
        model: '',
        seatsPerRow: 4,
        hasAirConditioning: true,
        hasWifi: false,
        isWheelchairAccessible: false,
      })
    }
  }, [editBus])

  const fetchBuses = async () => {
    try {
      const response = await busService.getAll()
      setBuses(response.data)
    } finally {
      setLoading(false)
    }
  }

  const handleSave = async () => {
    try {
      if (editBus) {
        await busService.update(editBus.id, formData)
      } else {
        await busService.create(formData)
      }
      await fetchBuses()
      setDialogOpen(false)
      setEditBus(null)
    } catch (error) {
      console.error('Failed to save bus:', error)
      alert('Failed to save bus')
    }
  }

  if (loading) return <LoadingSpinner />

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h6" fontWeight={600}>Fleet Management</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => setDialogOpen(true)}>
          Add Bus
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Plate Number</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Capacity</TableCell>
              <TableCell>Available</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {buses.map((bus) => (
              <TableRow key={bus.id}>
                <TableCell>{bus.plateNumber}</TableCell>
                <TableCell>{bus.type}</TableCell>
                <TableCell>{bus.capacity}</TableCell>
                <TableCell>{bus.availableSeats}</TableCell>
                <TableCell>
                  <Chip 
                    label={bus.status} 
                    color={bus.status === 'Active' ? 'success' : 'default'} 
                    size="small" 
                  />
                </TableCell>
                <TableCell>
                  <IconButton size="small" onClick={() => setEditBus(bus)}>
                    <Edit />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={dialogOpen || !!editBus} onClose={() => { setDialogOpen(false); setEditBus(null) }}>
        <DialogTitle>{editBus ? 'Edit Bus' : 'Add New Bus'}</DialogTitle>
        <DialogContent>
          <TextField 
            fullWidth 
            label="Plate Number" 
            sx={{ mt: 2, mb: 2 }} 
            value={formData.plateNumber}
            onChange={(e) => setFormData({ ...formData, plateNumber: e.target.value })}
            required
          />
          <TextField 
            fullWidth 
            label="Registration Number" 
            sx={{ mb: 2 }} 
            value={formData.registrationNumber}
            onChange={(e) => setFormData({ ...formData, registrationNumber: e.target.value })}
          />
          <TextField 
            fullWidth 
            label="Brand" 
            sx={{ mb: 2 }} 
            value={formData.brand}
            onChange={(e) => setFormData({ ...formData, brand: e.target.value })}
          />
          <TextField 
            fullWidth 
            label="Model" 
            sx={{ mb: 2 }} 
            value={formData.model}
            onChange={(e) => setFormData({ ...formData, model: e.target.value })}
          />
          <FormControl fullWidth sx={{ mb: 2 }}>
            <InputLabel>Type</InputLabel>
            <Select 
              value={formData.type} 
              label="Type"
              onChange={(e) => setFormData({ ...formData, type: e.target.value as number })}
            >
              <MenuItem value={0}>City</MenuItem>
              <MenuItem value={1}>Intercity</MenuItem>
              <MenuItem value={2}>Express</MenuItem>
            </Select>
          </FormControl>
          <TextField 
            fullWidth 
            type="number" 
            label="Capacity" 
            sx={{ mb: 2 }} 
            value={formData.capacity}
            onChange={(e) => setFormData({ ...formData, capacity: parseInt(e.target.value) || 0 })}
            required
          />
          <TextField 
            fullWidth 
            type="number" 
            label="Seats Per Row" 
            sx={{ mb: 2 }} 
            value={formData.seatsPerRow}
            onChange={(e) => setFormData({ ...formData, seatsPerRow: parseInt(e.target.value) || 4 })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { setDialogOpen(false); setEditBus(null) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>{editBus ? 'Update' : 'Create'}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function RouteManagement() {
  const [routes, setRoutes] = useState<Route[]>([])
  const [loading, setLoading] = useState(true)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editRoute, setEditRoute] = useState<Route | null>(null)
  const [formData, setFormData] = useState({
    name: '',
    origin: '',
    destination: '',
    distanceKm: 0,
    estimatedDurationMinutes: 0,
  })

  useEffect(() => {
    fetchRoutes()
  }, [])

  useEffect(() => {
    if (editRoute) {
      setFormData({
        name: editRoute.name || '',
        origin: editRoute.origin || '',
        destination: editRoute.destination || '',
        distanceKm: editRoute.distanceKm || 0,
        estimatedDurationMinutes: 0,
      })
    } else {
      setFormData({
        name: '',
        origin: '',
        destination: '',
        distanceKm: 0,
        estimatedDurationMinutes: 0,
      })
    }
  }, [editRoute])

  const fetchRoutes = async () => {
    try {
      const response = await routeService.getAll()
      setRoutes(response.data)
    } finally {
      setLoading(false)
    }
  }

  const handleSave = async () => {
    try {
      if (editRoute) {
        await routeService.update(editRoute.id, formData)
      } else {
        await routeService.create(formData)
      }
      await fetchRoutes()
      setDialogOpen(false)
      setEditRoute(null)
    } catch (error) {
      console.error('Failed to save route:', error)
      alert('Failed to save route')
    }
  }

  if (loading) return <LoadingSpinner />

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h6" fontWeight={600}>Route Management</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => setDialogOpen(true)}>
          Add Route
        </Button>
      </Box>

      <Grid container spacing={3}>
        {routes.map((route) => (
          <Grid item xs={12} sm={6} md={4} key={route.id}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                  <RouteIcon color="primary" />
                  <Typography variant="h6" fontWeight={600}>
                    {route.name}
                  </Typography>
                </Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {route.origin} → {route.destination}
                </Typography>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 2 }}>
                  <Typography variant="body2">
                    {route.distanceKm} km
                  </Typography>
                  <Box sx={{ display: 'flex', gap: 1 }}>
                    <IconButton size="small" onClick={() => setEditRoute(route)}>
                      <Edit />
                    </IconButton>
                    <Chip 
                      label={route.isActive ? 'Active' : 'Inactive'} 
                      color={route.isActive ? 'success' : 'default'} 
                      size="small" 
                    />
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Dialog open={dialogOpen || !!editRoute} onClose={() => { setDialogOpen(false); setEditRoute(null) }}>
        <DialogTitle>{editRoute ? 'Edit Route' : 'Add New Route'}</DialogTitle>
        <DialogContent>
          <TextField 
            fullWidth 
            label="Route Name" 
            sx={{ mt: 2, mb: 2 }} 
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="e.g., Tunis - Sfax Express"
          />
          <TextField 
            fullWidth 
            label="Origin City" 
            sx={{ mb: 2 }} 
            value={formData.origin}
            onChange={(e) => setFormData({ ...formData, origin: e.target.value })}
          />
          <TextField 
            fullWidth 
            label="Destination City" 
            sx={{ mb: 2 }} 
            value={formData.destination}
            onChange={(e) => setFormData({ ...formData, destination: e.target.value })}
          />
          <TextField 
            fullWidth 
            type="number" 
            label="Distance (km)" 
            sx={{ mb: 2 }} 
            value={formData.distanceKm}
            onChange={(e) => setFormData({ ...formData, distanceKm: parseFloat(e.target.value) })}
          />
          <TextField 
            fullWidth 
            type="number" 
            label="Duration (minutes)" 
            sx={{ mb: 2 }} 
            value={formData.estimatedDurationMinutes}
            onChange={(e) => setFormData({ ...formData, estimatedDurationMinutes: parseInt(e.target.value) })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { setDialogOpen(false); setEditRoute(null) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>{editRoute ? 'Update' : 'Create'}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

function ScheduleManagement() {
  const [schedules, setSchedules] = useState<Schedule[]>([])
  const [loading, setLoading] = useState(true)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editSchedule, setEditSchedule] = useState<Schedule | null>(null)
  const [buses, setBuses] = useState<Bus[]>([])
  const [routes, setRoutes] = useState<Route[]>([])
  const [formData, setFormData] = useState({
    busId: '',
    routeId: '',
    departureTime: '',
    arrivalTime: '',
    basePrice: 0,
    isRecurring: false,
    operatingDays: [] as number[],
  })

  useEffect(() => {
    fetchSchedules()
    fetchBuses()
    fetchRoutes()
  }, [])

  useEffect(() => {
    if (editSchedule) {
      setFormData({
        busId: editSchedule.busId || '',
        routeId: editSchedule.routeId || '',
        departureTime: editSchedule.departureTime.slice(0, 16),
        arrivalTime: editSchedule.arrivalTime.slice(0, 16),
        basePrice: editSchedule.basePrice || 0,
        isRecurring: editSchedule.isRecurring || false,
        operatingDays: editSchedule.operatingDays || [],
      })
    } else {
      setFormData({
        busId: '',
        routeId: '',
        departureTime: '',
        arrivalTime: '',
        basePrice: 0,
        isRecurring: false,
        operatingDays: [0, 1, 2, 3, 4, 5, 6],
      })
    }
  }, [editSchedule])

  const fetchSchedules = async () => {
    try {
      const response = await scheduleService.getAll()
      setSchedules(response.data)
    } finally {
      setLoading(false)
    }
  }

  const fetchBuses = async () => {
    try {
      const response = await busService.getAll()
      setBuses(response.data)
    } catch (error) {
      console.error('Failed to fetch buses:', error)
    }
  }

  const fetchRoutes = async () => {
    try {
      const response = await routeService.getAll()
      setRoutes(response.data)
    } catch (error) {
      console.error('Failed to fetch routes:', error)
    }
  }

  const handleSave = async () => {
    try {
      if (!formData.busId || !formData.routeId) {
        alert('Please select both a bus and a route')
        return
      }
      const payload = {
        busId: formData.busId,
        routeId: formData.routeId,
        departureTime: new Date(formData.departureTime).toISOString(),
        arrivalTime: new Date(formData.arrivalTime).toISOString(),
        basePrice: parseFloat(formData.basePrice.toString()),
        isRecurring: formData.isRecurring,
        operatingDays: formData.operatingDays,
      }
      if (editSchedule) {
        await scheduleService.update(editSchedule.id, payload)
      } else {
        await scheduleService.create(payload)
      }
      await fetchSchedules()
      setDialogOpen(false)
      setEditSchedule(null)
    } catch (error) {
      console.error('Failed to save schedule:', error)
      alert('Failed to save schedule')
    }
  }

  if (loading) return <LoadingSpinner />

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h6" fontWeight={600}>Schedule Management</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => setDialogOpen(true)}>
          Add Schedule
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Route</TableCell>
              <TableCell>Bus</TableCell>
              <TableCell>Departure</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Price</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {schedules.map((schedule) => (
              <TableRow key={schedule.id}>
                <TableCell>
                  {schedule.route.origin} → {schedule.route.destination}
                </TableCell>
                <TableCell>{schedule.bus.plateNumber}</TableCell>
                <TableCell>
                  {format(new Date(schedule.departureTime), 'MMM d, HH:mm')}
                </TableCell>
                <TableCell>
                  <Chip 
                    label={schedule.status} 
                    color={schedule.status === 'Scheduled' ? 'info' : schedule.status === 'Completed' ? 'success' : 'warning'} 
                    size="small" 
                  />
                </TableCell>
                <TableCell>{schedule.basePrice} TND</TableCell>
                <TableCell>
                  <IconButton size="small" onClick={() => setEditSchedule(schedule)}>
                    <Edit />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={dialogOpen || !!editSchedule} onClose={() => { setDialogOpen(false); setEditSchedule(null) }}>
        <DialogTitle>{editSchedule ? 'Edit Schedule' : 'Add New Schedule'}</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2, mb: 2 }}>
            <InputLabel>Route</InputLabel>
            <Select 
              value={formData.routeId} 
              label="Route"
              onChange={(e) => setFormData({ ...formData, routeId: e.target.value })}
            >
              {routes.map(route => (
                <MenuItem key={route.id} value={route.id}>
                  {route.name} ({route.origin} → {route.destination})
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth sx={{ mb: 2 }}>
            <InputLabel>Bus</InputLabel>
            <Select 
              value={formData.busId} 
              label="Bus"
              onChange={(e) => setFormData({ ...formData, busId: e.target.value })}
            >
              {buses.map(bus => (
                <MenuItem key={bus.id} value={bus.id}>
                  {bus.plateNumber} - {bus.type} ({bus.capacity} seats)
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField 
            fullWidth 
            type="datetime-local" 
            label="Departure Time" 
            sx={{ mb: 2 }} 
            value={formData.departureTime}
            onChange={(e) => setFormData({ ...formData, departureTime: e.target.value })}
            InputLabelProps={{ shrink: true }}
          />
          <TextField 
            fullWidth 
            type="datetime-local" 
            label="Arrival Time" 
            sx={{ mb: 2 }} 
            value={formData.arrivalTime}
            onChange={(e) => setFormData({ ...formData, arrivalTime: e.target.value })}
            InputLabelProps={{ shrink: true }}
          />
          <TextField 
            fullWidth 
            type="number" 
            label="Base Price (TND)" 
            sx={{ mb: 2 }} 
            value={formData.basePrice}
            onChange={(e) => setFormData({ ...formData, basePrice: parseFloat(e.target.value) })}
          />
          <FormControlLabel
            control={
              <Checkbox 
                checked={formData.isRecurring}
                onChange={(e) => setFormData({ ...formData, isRecurring: e.target.checked })}
              />
            }
            label="Recurring Schedule"
            sx={{ mb: 1 }}
          />
          {formData.isRecurring && (
            <Box sx={{ mb: 2 }}>
              <Typography variant="body2" sx={{ mb: 1 }}>Operating Days:</Typography>
              <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((day, index) => (
                  <FormControlLabel
                    key={index}
                    control={
                      <Checkbox 
                        checked={formData.operatingDays.includes(index)}
                        onChange={(e) => {
                          if (e.target.checked) {
                            setFormData({ ...formData, operatingDays: [...formData.operatingDays, index].sort() })
                          } else {
                            setFormData({ ...formData, operatingDays: formData.operatingDays.filter(d => d !== index) })
                          }
                        }}
                      />
                    }
                    label={day}
                  />
                ))}
              </Box>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { setDialogOpen(false); setEditSchedule(null) }}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>{editSchedule ? 'Update' : 'Create'}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default function AdminPage() {
  const location = useLocation()
  const [tab, setTab] = useState(0)

  const tabs = [
    { label: 'Buses', icon: <DirectionsBus />, path: '/admin/buses' },
    { label: 'Routes', icon: <RouteIcon />, path: '/admin/routes' },
    { label: 'Schedules', icon: <ScheduleIcon />, path: '/admin/schedules' },
    { label: 'Users', icon: <People />, path: '/admin/users' },
  ]

  useEffect(() => {
    const path = location.pathname
    if (path.includes('/buses')) setTab(0)
    else if (path.includes('/routes')) setTab(1)
    else if (path.includes('/schedules')) setTab(2)
    else if (path.includes('/users')) setTab(3)
  }, [location.pathname])

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      <Typography variant="h4" fontWeight={700} gutterBottom>
        Admin Panel
      </Typography>

      <Paper sx={{ mb: 3 }}>
        <Tabs value={tab} onChange={(_, v) => setTab(v)}>
          {tabs.map((t, i) => (
            <Tab 
              key={i} 
              label={t.label} 
              icon={t.icon} 
              iconPosition="start" 
              component={Link} 
              to={t.path}
            />
          ))}
        </Tabs>
      </Paper>

      <Routes>
        <Route path="/" element={<BusManagement />} />
        <Route path="/buses" element={<BusManagement />} />
        <Route path="/routes" element={<RouteManagement />} />
        <Route path="/schedules" element={<ScheduleManagement />} />
        <Route path="/users" element={<Box sx={{ p: 3 }}><Typography>User management coming soon</Typography></Box>} />
      </Routes>
    </Container>
  )
}
