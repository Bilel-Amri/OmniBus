import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import {
  AppBar,
  Box,
  Toolbar,
  Typography,
  Button,
  IconButton,
  Menu,
  MenuItem,
  Container,
  Avatar,
  Chip,
  useMediaQuery,
  useTheme,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  ListItemButton,
  Divider,
} from '@mui/material'
import {
  Menu as MenuIcon,
  DirectionsBus,
  ConfirmationNumber,
  Dashboard,
  AdminPanelSettings,
  Logout,
  LocalTaxi,
  SmartToy,
} from '@mui/icons-material'
import { useState } from 'react'
import { AIChatWidget } from '../AIChatWidget'

export default function Layout() {
  const { user, logout, isDriver, isAdmin } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const theme = useTheme()
  const isMobile = useMediaQuery(theme.breakpoints.down('md'))
  
  const [drawerOpen, setDrawerOpen] = useState(false)
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const [aiChatOpen, setAiChatOpen] = useState(false)

  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleClose = () => {
    setAnchorEl(null)
  }

  const handleLogout = () => {
    handleClose()
    logout()
    navigate('/login')
  }

  const menuItems = [
    { text: 'Home', icon: <LocalTaxi />, path: '/' },
    { text: 'Search Routes', icon: <DirectionsBus />, path: '/search' },
    { text: 'Live Map', icon: <DirectionsBus />, path: '/map' },
    { text: 'My Tickets', icon: <ConfirmationNumber />, path: '/my-tickets' },
    ...(isDriver ? [{ text: 'Driver Dashboard', icon: <Dashboard />, path: '/driver' }] : []),
    ...(isAdmin ? [
      { text: 'Admin Dashboard', icon: <AdminPanelSettings />, path: '/admin' },
      { text: 'Analytics', icon: <Dashboard />, path: '/dashboard' },
    ] : []),
  ]

  const drawer = (
    <Box sx={{ width: 250 }} role="presentation">
      <Box sx={{ p: 2, display: 'flex', alignItems: 'center', gap: 1 }}>
        <LocalTaxi sx={{ color: 'primary.main', fontSize: 32 }} />
        <Typography variant="h6" color="primary" fontWeight={700}>
          OmniBus
        </Typography>
      </Box>
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton
              selected={location.pathname === item.path}
              onClick={() => {
                navigate(item.path)
                setDrawerOpen(false)
              }}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  )

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static" elevation={0} sx={{ bgcolor: 'white', borderBottom: 1, borderColor: 'divider' }}>
        <Container maxWidth="xl">
          <Toolbar disableGutters>
            {isMobile && (
              <IconButton
                color="inherit"
                aria-label="menu"
                onClick={() => setDrawerOpen(true)}
                sx={{ mr: 2, color: 'text.primary' }}
              >
                <MenuIcon />
              </IconButton>
            )}
            
            <LocalTaxi sx={{ display: { xs: 'none', md: 'flex' }, mr: 1, color: 'primary.main' }} />
            <Typography
              variant="h6"
              noWrap
              component={Link}
              to="/"
              sx={{
                mr: 2,
                display: { xs: 'none', md: 'flex' },
                fontWeight: 700,
                color: 'primary.main',
                textDecoration: 'none',
                flexGrow: isMobile ? 1 : 0,
              }}
            >
              OmniBus
            </Typography>
            
            {!isMobile && (
              <Box sx={{ display: 'flex', gap: 1, ml: 3 }}>
                {menuItems.map((item) => (
                  <Button
                    key={item.text}
                    component={Link}
                    to={item.path}
                    startIcon={item.icon}
                    sx={{ 
                      color: location.pathname === item.path ? 'primary.main' : 'text.secondary',
                      bgcolor: location.pathname === item.path ? 'primary.50' : 'transparent',
                    }}
                  >
                    {item.text}
                  </Button>
                ))}
              </Box>
            )}
            
            <Box sx={{ flexGrow: 1 }} />
            
            {user ? (
              <>
                <Chip
                  avatar={<Avatar sx={{ bgcolor: 'primary.main' }}>{user.firstName[0]}</Avatar>}
                  label={`${user.firstName} ${user.lastName}`}
                  onClick={handleMenu}
                  sx={{ cursor: 'pointer' }}
                />
                <Menu
                  anchorEl={anchorEl}
                  open={Boolean(anchorEl)}
                  onClose={handleClose}
                  anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                  transformOrigin={{ vertical: 'top', horizontal: 'right' }}
                >
                  <MenuItem disabled>
                    <Typography variant="body2" color="text.secondary">
                      {user.role}
                    </Typography>
                  </MenuItem>
                  <Divider />
                  <MenuItem onClick={handleLogout}>
                    <ListItemIcon><Logout fontSize="small" /></ListItemIcon>
                    Logout
                  </MenuItem>
                </Menu>
              </>
            ) : (
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button component={Link} to="/login" variant="outlined" size="small">
                  Login
                </Button>
                <Button component={Link} to="/register" variant="contained" size="small">
                  Register
                </Button>
              </Box>
            )}
          </Toolbar>
        </Container>
      </AppBar>
      
      <Drawer anchor="left" open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        {drawer}
      </Drawer>
      
      <Box component="main" sx={{ flexGrow: 1 }}>
        <Outlet />
      </Box>
      
      {/* AI Chat Widget */}
      <AIChatWidget open={aiChatOpen} onClose={() => setAiChatOpen(false)} />
      
      {/* Floating AI Button */}
      {user && (
        <IconButton
          onClick={() => setAiChatOpen(true)}
          sx={{
            position: 'fixed',
            bottom: 24,
            right: 24,
            bgcolor: 'primary.main',
            color: 'white',
            width: 56,
            height: 56,
            boxShadow: 3,
            '&:hover': {
              bgcolor: 'primary.dark',
              transform: 'scale(1.1)',
            },
            transition: 'all 0.2s',
            zIndex: 1000,
          }}
        >
          <SmartToy sx={{ fontSize: 28 }} />
        </IconButton>
      )}
      
      <Box
        component="footer"
        sx={{
          py: 3,
          px: 2,
          mt: 'auto',
          bgcolor: 'background.paper',
          borderTop: 1,
          borderColor: 'divider',
        }}
      >
        <Container maxWidth="xl">
          <Typography variant="body2" color="text.secondary" align="center">
            © 2024 OmniBus - Bus Service Application for Tunisia. All rights reserved.
          </Typography>
        </Container>
      </Box>
    </Box>
  )
}
