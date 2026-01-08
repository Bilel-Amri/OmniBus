import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import Layout from './components/layout/Layout'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import HomePage from './pages/HomePage'
import SearchPage from './pages/SearchPage'
import BookingPage from './pages/BookingPage'
import MyTicketsPage from './pages/MyTicketsPage'
import DashboardPage from './pages/DashboardPage'
import DriverPage from './pages/DriverPage'
import AdminPage from './pages/AdminPage'
import StationsMapPage from './pages/StationsMapPage'
import LoadingSpinner from './components/common/LoadingSpinner'

function App() {
  const { user, loading } = useAuth()

  if (loading) {
    return <LoadingSpinner fullScreen />
  }

  return (
    <Routes>
      {/* Public routes */}
      <Route path="/login" element={user ? <Navigate to="/" /> : <LoginPage />} />
      <Route path="/register" element={user ? <Navigate to="/" /> : <RegisterPage />} />
      
      {/* Protected routes */}
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="search" element={<SearchPage />} />
        <Route path="map" element={<StationsMapPage />} />
        <Route path="booking/:scheduleId" element={<BookingPage />} />
        <Route path="my-tickets" element={<MyTicketsPage />} />
        
        {/* Driver routes */}
        <Route path="driver" element={user?.role === 'Driver' ? <DriverPage /> : <Navigate to="/" />} />
        
        {/* Admin routes */}
        <Route path="admin/*" element={user?.role === 'Admin' ? <AdminPage /> : <Navigate to="/" />} />
        <Route path="dashboard" element={user?.role === 'Admin' ? <DashboardPage /> : <Navigate to="/" />} />
      </Route>
      
      {/* Catch all */}
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  )
}

export default App
