import { useState, useEffect } from 'react';
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  CircularProgress,
} from '@mui/material';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import {
  TrendingUp,
  AttachMoney,
  People,
  DirectionsBus,
  Timeline,
} from '@mui/icons-material';

interface AnalyticsData {
  revenue: {
    totalRevenue: number;
    monthlyRevenue: number;
    dailyRevenue: number;
    growthRate: number;
    monthlyData: { month: string; revenue: number; bookings: number }[];
  };
  dailyBookings: { date: string; bookings: number; cancellations: number }[];
  routePopularity: {
    routeId: number;
    routeName: string;
    totalBookings: number;
    revenue: number;
    averageOccupancy: number;
  }[];
  occupancyRates: {
    scheduleId: number;
    routeName: string;
    departureTime: string;
    totalSeats: number;
    bookedSeats: number;
    occupancyPercentage: number;
  }[];
  systemStats: {
    totalUsers: number;
    totalDrivers: number;
    totalBuses: number;
    totalRoutes: number;
    activeSchedules: number;
    pendingPayments: number;
  };
}

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8', '#82CA9D'];

export default function AdminDashboardPage() {
  const [analytics, setAnalytics] = useState<AnalyticsData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchAnalytics();
  }, []);

  const fetchAnalytics = async () => {
    try {
      const response = await fetch('/api/analytics/dashboard', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
      const data = await response.json();
      setAnalytics(data);
    } catch (error) {
      console.error('Error fetching analytics:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="80vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!analytics) {
    return (
      <Container>
        <Typography>No analytics data available</Typography>
      </Container>
    );
  }

  const { revenue, dailyBookings, routePopularity, occupancyRates, systemStats } = analytics;

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" gutterBottom fontWeight="bold">
        📊 Admin Analytics Dashboard
      </Typography>

      {/* Key Metrics Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card sx={{ bgcolor: '#1976d2', color: 'white' }}>
            <CardContent>
              <Box display="flex" alignItems="center" justifyContent="space-between">
                <Box>
                  <Typography variant="h4" fontWeight="bold">
                    ${revenue.totalRevenue.toFixed(0)}
                  </Typography>
                  <Typography variant="body2">Total Revenue</Typography>
                </Box>
                <AttachMoney sx={{ fontSize: 48, opacity: 0.8 }} />
              </Box>
              <Box display="flex" alignItems="center" mt={1}>
                <TrendingUp sx={{ fontSize: 16, mr: 0.5 }} />
                <Typography variant="body2">
                  {revenue.growthRate > 0 ? '+' : ''}
                  {revenue.growthRate.toFixed(1)}% vs last month
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card sx={{ bgcolor: '#2e7d32', color: 'white' }}>
            <CardContent>
              <Box display="flex" alignItems="center" justifyContent="space-between">
                <Box>
                  <Typography variant="h4" fontWeight="bold">
                    {systemStats.totalUsers}
                  </Typography>
                  <Typography variant="body2">Total Users</Typography>
                </Box>
                <People sx={{ fontSize: 48, opacity: 0.8 }} />
              </Box>
              <Typography variant="body2" mt={1}>
                {systemStats.totalDrivers} drivers
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card sx={{ bgcolor: '#ed6c02', color: 'white' }}>
            <CardContent>
              <Box display="flex" alignItems="center" justifyContent="space-between">
                <Box>
                  <Typography variant="h4" fontWeight="bold">
                    {systemStats.activeSchedules}
                  </Typography>
                  <Typography variant="body2">Active Schedules</Typography>
                </Box>
                <Timeline sx={{ fontSize: 48, opacity: 0.8 }} />
              </Box>
              <Typography variant="body2" mt={1}>
                {systemStats.totalRoutes} routes available
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card sx={{ bgcolor: '#9c27b0', color: 'white' }}>
            <CardContent>
              <Box display="flex" alignItems="center" justifyContent="space-between">
                <Box>
                  <Typography variant="h4" fontWeight="bold">
                    {systemStats.totalBuses}
                  </Typography>
                  <Typography variant="body2">Total Buses</Typography>
                </Box>
                <DirectionsBus sx={{ fontSize: 48, opacity: 0.8 }} />
              </Box>
              <Typography variant="body2" mt={1}>
                {systemStats.pendingPayments} pending payments
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Charts */}
      <Grid container spacing={3}>
        {/* Revenue Trend */}
        <Grid item xs={12} lg={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom fontWeight="bold">
              📈 Revenue Trend (Last 6 Months)
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={revenue.monthlyData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="revenue"
                  stroke="#1976d2"
                  strokeWidth={2}
                  name="Revenue ($)"
                />
                <Line
                  type="monotone"
                  dataKey="bookings"
                  stroke="#2e7d32"
                  strokeWidth={2}
                  name="Bookings"
                />
              </LineChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Daily Bookings */}
        <Grid item xs={12} lg={4}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" gutterBottom fontWeight="bold">
              📅 Daily Performance
            </Typography>
            <Box sx={{ mt: 2 }}>
              <Typography variant="h3" fontWeight="bold" color="primary">
                ${revenue.dailyRevenue.toFixed(0)}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Today's Revenue
              </Typography>
              <Typography variant="h4" fontWeight="bold" color="success.main" sx={{ mt: 2 }}>
                ${revenue.monthlyRevenue.toFixed(0)}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                This Month
              </Typography>
            </Box>
          </Paper>
        </Grid>

        {/* Booking Trends */}
        <Grid item xs={12} lg={7}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom fontWeight="bold">
              📊 Booking Trends (Last 30 Days)
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={dailyBookings.slice(-30)}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="bookings" fill="#2e7d32" name="Bookings" />
                <Bar dataKey="cancellations" fill="#d32f2f" name="Cancellations" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Route Popularity */}
        <Grid item xs={12} lg={5}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom fontWeight="bold">
              🎯 Top Routes by Bookings
            </Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={routePopularity.slice(0, 6)}
                  dataKey="totalBookings"
                  nameKey="routeName"
                  cx="50%"
                  cy="50%"
                  outerRadius={100}
                  label={(entry) => `${entry.routeName}: ${entry.totalBookings}`}
                >
                  {routePopularity.slice(0, 6).map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        {/* Route Performance Table */}
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom fontWeight="bold">
              🚌 Route Performance Overview
            </Typography>
            <Box sx={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ borderBottom: '2px solid #ddd' }}>
                    <th style={{ textAlign: 'left', padding: '12px' }}>Route</th>
                    <th style={{ textAlign: 'center', padding: '12px' }}>Bookings</th>
                    <th style={{ textAlign: 'center', padding: '12px' }}>Revenue</th>
                    <th style={{ textAlign: 'center', padding: '12px' }}>Avg Occupancy</th>
                  </tr>
                </thead>
                <tbody>
                  {routePopularity.map((route, index) => (
                    <tr
                      key={route.routeId}
                      style={{
                        borderBottom: '1px solid #eee',
                        backgroundColor: index % 2 === 0 ? '#f9f9f9' : 'white',
                      }}
                    >
                      <td style={{ padding: '12px' }}>{route.routeName}</td>
                      <td style={{ textAlign: 'center', padding: '12px' }}>{route.totalBookings}</td>
                      <td style={{ textAlign: 'center', padding: '12px' }}>
                        ${route.revenue.toFixed(2)}
                      </td>
                      <td style={{ textAlign: 'center', padding: '12px' }}>
                        <span
                          style={{
                            color:
                              route.averageOccupancy > 70
                                ? '#2e7d32'
                                : route.averageOccupancy > 40
                                ? '#ed6c02'
                                : '#d32f2f',
                            fontWeight: 'bold',
                          }}
                        >
                          {route.averageOccupancy.toFixed(1)}%
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </Box>
          </Paper>
        </Grid>

        {/* Today's Occupancy */}
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom fontWeight="bold">
              🎫 Today's Schedule Occupancy
            </Typography>
            <Box sx={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ borderBottom: '2px solid #ddd' }}>
                    <th style={{ textAlign: 'left', padding: '12px' }}>Route</th>
                    <th style={{ textAlign: 'center', padding: '12px' }}>Departure</th>
                    <th style={{ textAlign: 'center', padding: '12px' }}>Booked/Total</th>
                    <th style={{ textAlign: 'center', padding: '12px' }}>Occupancy</th>
                  </tr>
                </thead>
                <tbody>
                  {occupancyRates.map((schedule, index) => (
                    <tr
                      key={schedule.scheduleId}
                      style={{
                        borderBottom: '1px solid #eee',
                        backgroundColor: index % 2 === 0 ? '#f9f9f9' : 'white',
                      }}
                    >
                      <td style={{ padding: '12px' }}>{schedule.routeName}</td>
                      <td style={{ textAlign: 'center', padding: '12px' }}>
                        {new Date(schedule.departureTime).toLocaleTimeString('en-US', {
                          hour: '2-digit',
                          minute: '2-digit',
                        })}
                      </td>
                      <td style={{ textAlign: 'center', padding: '12px' }}>
                        {schedule.bookedSeats}/{schedule.totalSeats}
                      </td>
                      <td style={{ textAlign: 'center', padding: '12px' }}>
                        <Box
                          component="span"
                          sx={{
                            display: 'inline-block',
                            width: '100px',
                            bgcolor: '#e0e0e0',
                            borderRadius: '4px',
                            overflow: 'hidden',
                          }}
                        >
                          <Box
                            sx={{
                              width: `${schedule.occupancyPercentage}%`,
                              bgcolor:
                                schedule.occupancyPercentage > 80
                                  ? '#2e7d32'
                                  : schedule.occupancyPercentage > 50
                                  ? '#ed6c02'
                                  : '#1976d2',
                              height: '20px',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              color: 'white',
                              fontSize: '12px',
                              fontWeight: 'bold',
                            }}
                          >
                            {schedule.occupancyPercentage.toFixed(0)}%
                          </Box>
                        </Box>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
