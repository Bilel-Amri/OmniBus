import { useState, useEffect } from 'react'
import {
  Box,
  Container,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  CircularProgress,
  Alert,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Chip,
} from '@mui/material'
import {
  TrendingUp,
  People,
  AttachMoney,
  ConfirmationNumber,
} from '@mui/icons-material'
import { ticketService, paymentService } from '../services/api'

interface TicketStats {
  totalTickets: number
  completedTickets: number
  cancelledTickets: number
  pendingTickets: number
  totalRevenue: number
  todayRevenue: number
}

interface PaymentStats {
  totalRevenue: number
  todayRevenue: number
  thisMonthRevenue: number
  totalPayments: number
  successfulPayments: number
  failedPayments: number
  refundedPayments: number
  totalRefunded: number
}

export default function DashboardPage() {
  const [ticketStats, setTicketStats] = useState<TicketStats | null>(null)
  const [paymentStats, setPaymentStats] = useState<PaymentStats | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchStats()
  }, [])

  const fetchStats = async () => {
    try {
      const [ticketRes, paymentRes] = await Promise.all([
        ticketService.getStats(),
        paymentService.getStats(),
      ])
      setTicketStats(ticketRes.data)
      setPaymentStats(paymentRes.data)
    } catch (err: any) {
      setError('Failed to load statistics')
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <Container maxWidth="xl" sx={{ py: 8, textAlign: 'center' }}>
        <CircularProgress />
      </Container>
    )
  }

  if (error) {
    return (
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    )
  }

  const statCards = [
    { 
      title: 'Total Revenue', 
      value: `${paymentStats?.totalRevenue.toFixed(2) || 0} TND`, 
      icon: <AttachMoney />, 
      color: 'success.main' 
    },
    { 
      title: "Today's Revenue", 
      value: `${paymentStats?.todayRevenue.toFixed(2) || 0} TND`, 
      icon: <TrendingUp />, 
      color: 'primary.main' 
    },
    { 
      title: 'Total Tickets', 
      value: ticketStats?.totalTickets || 0, 
      icon: <ConfirmationNumber />, 
      color: 'info.main' 
    },
    { 
      title: 'Completed Trips', 
      value: ticketStats?.completedTickets || 0, 
      icon: <People />, 
      color: 'warning.main' 
    },
  ]

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      <Typography variant="h4" fontWeight={700} gutterBottom>
        Admin Dashboard
      </Typography>

      {/* Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {statCards.map((stat, index) => (
          <Grid item xs={12} sm={6} md={3} key={index}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                  <Box
                    sx={{
                      p: 1.5,
                      borderRadius: 2,
                      bgcolor: `${stat.color}15`,
                      color: stat.color,
                    }}
                  >
                    {stat.icon}
                  </Box>
                  <Box>
                    <Typography variant="body2" color="text.secondary">
                      {stat.title}
                    </Typography>
                    <Typography variant="h5" fontWeight={700}>
                      {stat.value}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Additional Stats */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" fontWeight={600} gutterBottom>
              Ticket Overview
            </Typography>
            <TableContainer>
              <Table>
                <TableBody>
                  <TableRow>
                    <TableCell>Pending Tickets</TableCell>
                    <TableCell align="right">
                      <Chip label={ticketStats?.pendingTickets || 0} color="warning" size="small" />
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>Completed Tickets</TableCell>
                    <TableCell align="right">
                      <Chip label={ticketStats?.completedTickets || 0} color="success" size="small" />
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>Cancelled Tickets</TableCell>
                    <TableCell align="right">
                      <Chip label={ticketStats?.cancelledTickets || 0} color="error" size="small" />
                    </TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" fontWeight={600} gutterBottom>
              Payment Overview
            </Typography>
            <TableContainer>
              <Table>
                <TableBody>
                  <TableRow>
                    <TableCell>Successful Payments</TableCell>
                    <TableCell align="right">
                      <Chip label={paymentStats?.successfulPayments || 0} color="success" size="small" />
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>Failed Payments</TableCell>
                    <TableCell align="right">
                      <Chip label={paymentStats?.failedPayments || 0} color="error" size="small" />
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>Refunded Payments</TableCell>
                    <TableCell align="right">
                      <Chip label={paymentStats?.refundedPayments || 0} color="warning" size="small" />
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>Total Refunded</TableCell>
                    <TableCell align="right">
                      {paymentStats?.totalRefunded.toFixed(2) || 0} TND
                    </TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  )
}
