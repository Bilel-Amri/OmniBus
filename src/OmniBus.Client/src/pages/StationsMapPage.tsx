import { useEffect, useMemo, useState } from 'react'
import {
  Box, 
  Chip, 
  Container, 
  Paper, 
  Stack, 
  Typography, 
  Alert, 
  CircularProgress,
  FormGroup,
  FormControlLabel,
  Checkbox,
  Card,
  CardContent,
  Divider,
  TextField,
  InputAdornment,
  IconButton,
  Badge
} from '@mui/material'
import { MapContainer, TileLayer, Marker, Popup, CircleMarker, Polyline, Tooltip } from 'react-leaflet'
import L from 'leaflet'
import { routeService, scheduleService } from '../services/api'
import { signalRService } from '../services/signalR'
import { DirectionsBus, Place, Search, Clear, MyLocation } from '@mui/icons-material'

// Create custom bus icon using divIcon with Material-UI icon
const createBusIcon = (color: string) => L.divIcon({
  className: 'custom-bus-icon',
  html: `<div style="color: ${color}; font-size: 24px; filter: drop-shadow(0 2px 4px rgba(0,0,0,0.4));">
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="28" height="28" fill="currentColor">
      <path d="M4 16c0 .88.39 1.67 1 2.22V20c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h8v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1.78c.61-.55 1-1.34 1-2.22V6c0-3.5-3.58-4-8-4s-8 .5-8 4v10zm3.5 1c-.83 0-1.5-.67-1.5-1.5S6.67 14 7.5 14s1.5.67 1.5 1.5S8.33 17 7.5 17zm9 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm1.5-6H6V6h12v5z"/>
    </svg>
  </div>`,
  iconSize: [28, 28],
  iconAnchor: [14, 14],
  popupAnchor: [0, -14],
})

// Route color palette - vibrant colors for better visibility
const ROUTE_COLORS = [
  '#FF6B6B', '#4ECDC4', '#45B7D1', '#FFA07A', '#98D8C8',
  '#F7DC6F', '#BB8FCE', '#85C1E2', '#F8B739', '#52B788',
  '#E63946', '#06FFA5', '#FFB703', '#8338EC', '#3A86FF',
  '#FB5607', '#06D6A0', '#FF006E', '#8338EC', '#FFBE0B'
]

const stationColor = '#7C3AED'

interface Stop {
  id: string
  name: string
  latitude: number
  longitude: number
  routeName: string
  routeId: string
  order: number
}

interface RouteWithStops {
  id: string
  name: string
  origin: string
  destination: string
  stops: Stop[]
  color: string
}

interface BusMarker {
  id: string
  routeId: string
  routeName: string
  status: string
  latitude: number
  longitude: number
  updatedAt?: string
  color: string
}

export default function StationsMapPage() {
  const [routes, setRoutes] = useState<RouteWithStops[]>([])
  const [buses, setBuses] = useState<BusMarker[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [visibleRoutes, setVisibleRoutes] = useState<Set<string>>(new Set())
  const [selectedStation, setSelectedStation] = useState<Stop | null>(null)
  const [searchQuery, setSearchQuery] = useState('')

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [routesRes, schedulesRes] = await Promise.all([
          routeService.getAll(), // Changed from getActive() to getAll() to show all routes
          scheduleService.getActiveWithLocation(),
        ])

        const routesData: any[] = Array.isArray(routesRes.data) ? routesRes.data : []
        const schedules: any[] = Array.isArray(schedulesRes.data) ? schedulesRes.data : []

        // Build routes with stops and assign colors
        const processedRoutes: RouteWithStops[] = routesData
          .map((route: any, index) => {
            const stopsArr = Array.isArray(route.stops) ? route.stops : []
            const stops: Stop[] = stopsArr
              .filter((stop: any) => 
                typeof stop.latitude === 'number' && 
                typeof stop.longitude === 'number'
              )
              .map((stop: any) => ({
                id: stop.id,
                name: stop.name,
                latitude: stop.latitude,
                longitude: stop.longitude,
                routeName: route.name,
                routeId: route.id,
                order: stop.order || 0,
              }))
              .sort((a, b) => a.order - b.order)

            return {
              id: route.id,
              name: route.name,
              origin: route.origin,
              destination: route.destination,
              stops,
              color: ROUTE_COLORS[index % ROUTE_COLORS.length],
            }
          })
          .filter(r => r.stops.length > 0)

        setRoutes(processedRoutes)
        setVisibleRoutes(new Set(processedRoutes.map(r => r.id)))

        // Show info about routes without stops
        const routesWithoutStops = routesData.length - processedRoutes.length
        if (routesWithoutStops > 0) {
          console.info(`📍 Loaded ${processedRoutes.length} routes with stops. ${routesWithoutStops} routes don't have stop coordinates yet.`)
        }

        // Create route color map for buses
        const routeColorMap = new Map<string, string>()
        processedRoutes.forEach(r => routeColorMap.set(r.id, r.color))

        const mappedBuses: BusMarker[] = schedules
          .filter(
            (s: any) =>
              s && s.currentLocation &&
              typeof s.currentLocation.latitude === 'number' &&
              typeof s.currentLocation.longitude === 'number' &&
              s.routeId
          )
          .map((s: any) => ({
            id: s.id,
            routeId: s.routeId,
            routeName: s.route ? `${s.route.origin} → ${s.route.destination}` : 'Route',
            status: s.status,
            latitude: s.currentLocation.latitude,
            longitude: s.currentLocation.longitude,
            updatedAt: s.currentLocation.timestamp,
            color: routeColorMap.get(s.routeId) || '#0F766E',
          }))
        setBuses(mappedBuses)
      } catch (err: any) {
        setError('Failed to load map data')
      } finally {
        setLoading(false)
      }
    }

    fetchData()
  }, [])

  useEffect(() => {
    let isMounted = true
    let connectionAttempted = false
    
    const connect = async () => {
      if (connectionAttempted) return
      connectionAttempted = true
      
      try {
        await signalRService.connectToTrackingHub()
        if (!isMounted) return
        
        await signalRService.joinAdmin()
        signalRService.onBusLocationUpdated((scheduleId, location) => {
          if (!isMounted || !location) return
          setBuses((prev) => {
            const existing = prev.find((b) => b.id === scheduleId)
            if (existing && typeof location.latitude === 'number' && typeof location.longitude === 'number') {
              return prev.map((b) =>
                b.id === scheduleId
                  ? { ...b, latitude: location.latitude, longitude: location.longitude, updatedAt: location.timestamp as any }
                  : b
              )
            }
            return prev
          })
        })
      } catch (err) {
        // Only log if component is still mounted
        if (isMounted) {
          console.error('SignalR connection failed', err)
        }
      }
    }

    connect()

    return () => {
      isMounted = false
      signalRService.disconnect()
    }
  }, [])

  const handleRouteToggle = (routeId: string) => {
    setVisibleRoutes(prev => {
      const newSet = new Set(prev)
      if (newSet.has(routeId)) {
        newSet.delete(routeId)
      } else {
        newSet.add(routeId)
      }
      return newSet
    })
  }

  const handleStationClick = (station: Stop) => {
    setSelectedStation(station)
    // Find and highlight routes that pass through this station
    const routesForStation = routes.filter(route => 
      route.stops.some(stop => stop.id === station.id)
    ).map(r => r.id)
    setVisibleRoutes(new Set(routesForStation))
  }

  const handleClearSelection = () => {
    setSelectedStation(null)
    setSearchQuery('')
    setVisibleRoutes(new Set(routes.map(r => r.id)))
  }

  const filteredRoutes = useMemo(() => {
    if (!searchQuery) return routes
    const query = searchQuery.toLowerCase()
    return routes.filter(route => 
      route.name.toLowerCase().includes(query) ||
      route.origin.toLowerCase().includes(query) ||
      route.destination.toLowerCase().includes(query) ||
      route.stops.some(stop => stop.name.toLowerCase().includes(query))
    )
  }, [routes, searchQuery])

  const allStops = useMemo(() => {
    const stopsMap = new Map<string, Stop>()
    routes.forEach(route => {
      if (visibleRoutes.has(route.id)) {
        route.stops.forEach(stop => {
          const key = `${stop.latitude}_${stop.longitude}`
          if (!stopsMap.has(key)) {
            stopsMap.set(key, stop)
          }
        })
      }
    })
    return Array.from(stopsMap.values())
  }, [routes, visibleRoutes])

  const visibleBuses = useMemo(() => 
    buses.filter(bus => visibleRoutes.has(bus.routeId)),
    [buses, visibleRoutes]
  )

  if (loading) {
    return (
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '70vh' }}>
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      <Stack spacing={3}>
        <Box>
          <Typography variant="h4" fontWeight={700} gutterBottom>
            🗺️ Tunisia Network Map
          </Typography>
          <Typography color="text.secondary">
            Interactive route visualization with live bus tracking across Tunisia (auto-updates in real time)
          </Typography>
        </Box>

        {error && <Alert severity="error">{error}</Alert>}

        <Stack direction={{ xs: 'column', lg: 'row' }} spacing={2}>
          {/* Route Filter Sidebar */}
          <Card sx={{ minWidth: 280, maxWidth: { xs: '100%', lg: 280 } }}>
            <CardContent>
              <Stack spacing={2}>
                <Box>
                  <Typography variant="h6" fontWeight={600} gutterBottom>
                    📍 Routes with Stops ({routes.length})
                  </Typography>
                  <Typography variant="caption" color="text.secondary" display="block">
                    Showing routes that have station coordinates
                  </Typography>
                </Box>

                {selectedStation && (
                  <Alert 
                    severity="info" 
                    onClose={handleClearSelection}
                    sx={{ py: 0.5 }}
                  >
                    <Typography variant="caption" fontWeight={600}>
                      📍 {selectedStation.name}
                    </Typography>
                    <Typography variant="caption" display="block">
                      Showing {filteredRoutes.length} route(s)
                    </Typography>
                  </Alert>
                )}

                <TextField
                  size="small"
                  placeholder="Search routes or stations..."
                  value={searchQuery}
                  onChange={(e) => {
                    setSearchQuery(e.target.value)
                    if (e.target.value) {
                      const filtered = routes.filter(route => {
                        const query = e.target.value.toLowerCase()
                        return route.name.toLowerCase().includes(query) ||
                          route.origin.toLowerCase().includes(query) ||
                          route.destination.toLowerCase().includes(query) ||
                          route.stops.some(stop => stop.name.toLowerCase().includes(query))
                      })
                      setVisibleRoutes(new Set(filtered.map(r => r.id)))
                    } else {
                      setVisibleRoutes(new Set(routes.map(r => r.id)))
                    }
                  }}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Search sx={{ fontSize: 20 }} />
                      </InputAdornment>
                    ),
                    endAdornment: searchQuery && (
                      <InputAdornment position="end">
                        <IconButton size="small" onClick={() => {
                          setSearchQuery('')
                          setVisibleRoutes(new Set(routes.map(r => r.id)))
                        }}>
                          <Clear sx={{ fontSize: 18 }} />
                        </IconButton>
                      </InputAdornment>
                    )
                  }}
                />

                <Divider />

                <FormGroup sx={{ maxHeight: 400, overflow: 'auto' }}>
                  {filteredRoutes.map(route => (
                  <FormControlLabel
                    key={route.id}
                    control={
                      <Checkbox
                        checked={visibleRoutes.has(route.id)}
                        onChange={() => handleRouteToggle(route.id)}
                        size="small"
                      />
                    }
                    label={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Box
                          sx={{
                            width: 12,
                            height: 12,
                            borderRadius: '50%',
                            bgcolor: route.color,
                            flexShrink: 0,
                          }}
                        />
                        <Typography variant="body2" noWrap>
                          {route.name}
                        </Typography>
                      </Box>
                    }
                  />
                ))}
                </FormGroup>

                {filteredRoutes.length === 0 && (
                  <Typography variant="body2" color="text.secondary" textAlign="center" sx={{ py: 2 }}>
                    {searchQuery ? 'No routes match your search' : 'No routes with stops found'}
                  </Typography>
                )}
              </Stack>
            </CardContent>
          </Card>

          {/* Map Container */}
          <Paper sx={{ flex: 1, height: 600, overflow: 'hidden', position: 'relative' }}>
            <MapContainer
              style={{ height: '100%', width: '100%' }}
              center={[34.8, 10.1]}
              zoom={6}
              scrollWheelZoom
            >
              <TileLayer
                attribution="&copy; OpenStreetMap contributors"
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              />

              {/* Draw route lines */}
              {routes.map(route => {
                if (!visibleRoutes.has(route.id) || route.stops.length < 2) return null
                
                const positions: [number, number][] = route.stops.map(stop => [
                  stop.latitude,
                  stop.longitude,
                ])

                return (
                  <Polyline
                    key={route.id}
                    positions={positions}
                    pathOptions={{
                      color: route.color,
                      weight: 4,
                      opacity: 0.7,
                      lineCap: 'round',
                      lineJoin: 'round',
                    }}
                  >
                    <Tooltip permanent={false} direction="top">
                      <Typography variant="caption" fontWeight={600}>
                        {route.name}
                      </Typography>
                    </Tooltip>
                  </Polyline>
                )
              })}

              {/* Station markers */}
              {allStops.map((stop) => (
                <CircleMarker
                  key={stop.id}
                  center={[stop.latitude, stop.longitude]}
                  radius={selectedStation?.id === stop.id ? 12 : 8}
                  pathOptions={{
                    color: selectedStation?.id === stop.id ? '#FFD700' : '#FFFFFF',
                    weight: selectedStation?.id === stop.id ? 3 : 2,
                    fillColor: selectedStation?.id === stop.id ? '#FF6B6B' : stationColor,
                    fillOpacity: 0.9,
                  }}
                  eventHandlers={{
                    click: () => handleStationClick(stop)
                  }}
                >
                  <Popup>
                    <Box sx={{ minWidth: 150 }}>
                      <Typography fontWeight={700} gutterBottom>
                        📍 {stop.name}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        Route: {stop.routeName}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        Stop #{stop.order + 1}
                      </Typography>
                    </Box>
                  </Popup>
                  <Tooltip direction="top" offset={[0, -10]}>
                    <Typography variant="caption">{stop.name}</Typography>
                  </Tooltip>
                </CircleMarker>
              ))}

              {/* Bus markers */}
              {visibleBuses.map((bus) => (
                <Marker
                  key={bus.id}
                  position={[bus.latitude, bus.longitude]}
                  icon={createBusIcon(bus.color)}
                >
                  <Popup>
                    <Box sx={{ minWidth: 180 }}>
                      <Typography fontWeight={700} gutterBottom>
                        🚌 {bus.routeName}
                      </Typography>
                      <Stack spacing={0.5}>
                        <Chip 
                          label={bus.status} 
                          size="small"
                          color={bus.status === 'InProgress' ? 'success' : 'default'}
                          sx={{ width: 'fit-content' }}
                        />
                        {bus.updatedAt && (
                          <Typography variant="caption" color="text.secondary">
                            ⏱️ Updated: {new Date(bus.updatedAt).toLocaleTimeString()}
                          </Typography>
                        )}
                      </Stack>
                    </Box>
                  </Popup>
                </Marker>
              ))}
            </MapContainer>

            {/* Map Legend */}
            <Box
              sx={{
                position: 'absolute',
                bottom: 16,
                right: 16,
                bgcolor: 'rgba(255, 255, 255, 0.95)',
                p: 2,
                borderRadius: 2,
                boxShadow: 3,
                zIndex: 1000,
              }}
            >
              <Typography variant="caption" fontWeight={600} display="block" gutterBottom>
                MAP LEGEND
              </Typography>
              <Stack spacing={1}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Box
                    sx={{
                      width: 16,
                      height: 16,
                      borderRadius: '50%',
                      bgcolor: stationColor,
                      border: '2px solid white',
                    }}
                  />
                  <Typography variant="caption">Station</Typography>
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <DirectionsBus sx={{ fontSize: 16, color: ROUTE_COLORS[0] }} />
                  <Typography variant="caption">Live Bus</Typography>
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Box
                    sx={{
                      width: 24,
                      height: 3,
                      bgcolor: ROUTE_COLORS[0],
                      borderRadius: 1,
                    }}
                  />
                  <Typography variant="caption">Route Path</Typography>
                </Box>
              </Stack>
            </Box>
          </Paper>
        </Stack>

        {/* Statistics */}
        <Stack direction="row" spacing={2} flexWrap="wrap">
          <Chip
            icon={<Place />}
            label={`${allStops.length} Stations`}
            sx={{ bgcolor: '#F3E8FF', color: stationColor, fontWeight: 600 }}
          />
          <Chip
            icon={<DirectionsBus />}
            label={`${visibleBuses.length} Active Buses`}
            color="success"
            sx={{ fontWeight: 600 }}
          />
          <Typography variant="body2" color="text.secondary" sx={{ py: 0.5 }}>
            Real-time updates via SignalR • Click routes to show/hide • Hover for details
          </Typography>
        </Stack>
      </Stack>
    </Container>
  )
}

function dedupeStops(stops: Stop[]): Stop[] {
  const seen = new Map<string, Stop>()
  stops.forEach((s) => {
    const key = `${s.name.toLowerCase()}_${s.latitude.toFixed(4)}_${s.longitude.toFixed(4)}`
    if (!seen.has(key)) {
      seen.set(key, s)
    }
  })
  return Array.from(seen.values())
}
