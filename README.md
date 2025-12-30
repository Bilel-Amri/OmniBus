# OmniBus - Bus Service Application for Tunisia

A comprehensive, production-ready bus service platform with real-time tracking, online ticket booking, and administrative management for Tunisia's public transportation system.

## 🚌 Features

### Passenger Features
- **User Authentication**: Secure registration and login with JWT tokens
- **Route Search**: Search city and intercity bus routes
- **Seat Selection**: Interactive seat map with real-time availability
- **Online Booking**: Book tickets with secure payment processing
- **Real-Time Tracking**: Live bus location tracking via SignalR
- **Booking History**: View past and upcoming trips
- **Notifications**: Receive updates on arrival times, delays, and trip changes

### Driver Features
- **Location Updates**: Update real-time bus location via mobile app
- **Trip Management**: View assigned schedules and routes
- **Boarding Confirmation**: Confirm passenger boarding with ticket validation

### Admin Features
- **Fleet Management**: CRUD operations for buses, routes, and schedules
- **Real-Time Dashboard**: Monitor all buses on live map
- **Reports & Analytics**: Revenue reports, occupancy rates, and operational metrics
- **User Management**: Manage passengers, drivers, and staff

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8 Web API
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT with Role-based Access Control
- **Real-Time**: SignalR for live updates
- **Architecture**: Clean Architecture (Domain → Application → Infrastructure → API)

### Frontend
- **Framework**: React 18 with TypeScript
- **State Management**: React Query + Context API
- **UI Library**: Material UI (MUI)
- **Maps**: Leaflet (OpenStreetMap)
- **Real-Time**: @microsoft/signalr-client

## 📁 Project Structure

```
OmniBus/
├── src/
│   ├── OmniBus.Domain/           # Domain layer - Entities, Enums, Interfaces
│   ├── OmniBus.Application/      # Application layer - DTOs, CQRS, Services
│   ├── OmniBus.Infrastructure/   # Infrastructure - Database, Auth, External Services
│   ├── OmniBus.API/              # API Layer - Controllers, SignalR Hubs
│   └── OmniBus.Client/           # React Frontend Application
├── docs/                         # Documentation
├── scripts/                      # Build and deployment scripts
└── OmniBus.sln                  # Visual Studio Solution File
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- PostgreSQL 14+
- Visual Studio 2022 or VS Code

### Backend Setup

1. **Restore packages**:
```bash
cd src/OmniBus.API
dotnet restore
```

2. **Configure database**:
Update `appsettings.json` with your PostgreSQL connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=OmniBusDb;Username=postgres;Password=yourpassword"
}
```

3. **Run the application**:
```bash
dotnet run
```

API will be available at `https://localhost:5001` or `http://localhost:5000`

### Frontend Setup

1. **Install dependencies**:
```bash
cd src/OmniBus.Client
npm install
```

2. **Start development server**:
```bash
npm run dev
```

Frontend will be available at `http://localhost:5173`

## 📖 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register new user | Public |
| POST | `/api/auth/login` | Login and get JWT | Public |
| GET | `/api/auth/me` | Get current user | All |
| PUT | `/api/auth/profile` | Update profile | All |
| POST | `/api/auth/change-password` | Change password | All |

### Route Endpoints

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| GET | `/api/routes` | Get all routes | Public |
| GET | `/api/routes/{id}` | Get route by ID | Public |
| GET | `/api/routes/search` | Search routes | Public |
| GET | `/api/routes/active` | Get active routes | Public |
| POST | `/api/routes` | Create route | Admin |
| PUT | `/api/routes/{id}` | Update route | Admin |
| DELETE | `/api/routes/{id}` | Delete route | Admin |

### Schedule Endpoints

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| GET | `/api/schedules` | Get all schedules | Public |
| GET | `/api/schedules/{id}` | Get schedule details | Public |
| GET | `/api/schedules/search` | Search schedules | Public |
| GET | `/api/schedules/{id}/seats` | Get seat layout | Public |
| GET | `/api/schedules/active` | Get active with location | Public |
| POST | `/api/schedules` | Create schedule | Admin |
| PUT | `/api/schedules/{id}` | Update schedule | Admin |
| PUT | `/api/schedules/{id}/location` | Update location | Driver |

### Ticket Endpoints

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| GET | `/api/tickets/my-tickets` | Get my tickets | Passenger |
| GET | `/api/tickets/upcoming` | Get upcoming tickets | Passenger |
| GET | `/api/tickets/history` | Get ticket history | Passenger |
| POST | `/api/tickets/lock-seat` | Lock a seat | Passenger |
| POST | `/api/tickets/book` | Book a ticket | Passenger |
| POST | `/api/tickets/{id}/cancel` | Cancel ticket | Passenger |
| GET | `/api/tickets/stats` | Get ticket statistics | Admin |

### Driver Endpoints

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| GET | `/api/driver/trips` | Get assigned trips | Driver |
| GET | `/api/driver/today` | Get today's trips | Driver |
| PUT | `/api/driver/location` | Update location | Driver |
| POST | `/api/driver/confirm-boarding` | Confirm boarding | Driver |
| PUT | `/api/driver/schedule/{id}/start` | Start trip | Driver |
| PUT | `/api/driver/schedule/{id}/complete` | Complete trip | Driver |

### Payment Endpoints

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| POST | `/api/payments/process` | Process payment | Passenger |
| GET | `/api/payments/my-payments` | Get my payments | Passenger |
| GET | `/api/payments/stats` | Get payment stats | Admin |
| POST | `/api/payments/{id}/refund` | Refund payment | Admin |

## 🔐 Role-Based Access

- **Passenger**: Can search routes, book tickets, view history
- **Driver**: Can update location, view trips, confirm boarding
- **Admin**: Full access to all resources and admin features

## 📡 SignalR Hubs

### Tracking Hub
- **Endpoint**: `/hubs/tracking`
- **Methods**:
  - `JoinRoute(routeId)`: Subscribe to route updates
  - `LeaveRoute(routeId)`: Unsubscribe from route updates
  - `JoinSchedule(scheduleId)`: Subscribe to schedule updates
  - `JoinAdmin()`: Subscribe to all updates (admin)

### Events:
- `BusLocationUpdated`: Broadcast when bus location changes
- `ScheduleStatusChanged`: Broadcast when trip status changes
- `DelayNotification`: Broadcast when there's a delay

### Booking Hub
- **Endpoint**: `/hubs/booking`
- **Methods**:
  - `JoinSchedule(scheduleId)`: Subscribe to seat updates

### Events:
- `SeatLocked`: Seat is being booked
- `SeatBooked`: Seat booking confirmed
- `SeatLockReleased`: Seat lock expired
- `AvailabilityChanged`: Available seats count changed

## 🗺️ Map Integration

The application uses OpenStreetMap via Leaflet for:
- Interactive route maps
- Real-time bus tracking
- Stop location visualization

## 💳 Payment Integration

Supports integration with payment gateways:
- Stripe
- PayPal
- Local Tunisian payment gateways (e.g., D17, Click2Pay)

## 🧪 Testing

### Unit Tests
```bash
cd OmniBus.Tests
dotnet test
```

### Integration Tests
```bash
cd OmniBus.Tests
dotnet test --filter "Category=Integration"
```

## 🐳 Docker Deployment

```bash
docker-compose up -d
```

## ☁️ Cloud Deployment

### Azure App Service
1. Create Azure App Service (ASP.NET Core 8)
2. Create Azure Database for PostgreSQL
3. Configure environment variables
4. Deploy using GitHub Actions or Azure DevOps

### AWS
1. Deploy API to AWS Elastic Beanstalk
2. Use Amazon RDS for PostgreSQL
3. Configure Application Load Balancer

## 📁 Deliverables

1. **Backend Solution**: Complete .NET solution with:
   - Domain layer with all entities and interfaces
   - Application layer with DTOs and services
   - Infrastructure layer with EF Core DbContext and repositories
   - API layer with controllers and SignalR hubs

2. **Database Schema**: PostgreSQL schema with seed data for:
   - Admin user
   - Sample drivers
   - Sample buses
   - Sample routes (Tunis-Sfax, Tunis-Sousse, Tunis-Bizerte)
   - Sample schedules

3. **Frontend Application**: React application with:
   - Home page with route search
   - Search results page
   - Seat selection and booking
   - My tickets page
   - Driver dashboard
   - Admin panel

4. **Real-Time Features**: SignalR implementation for:
   - Bus location tracking
   - Seat availability updates
   - Delay notifications

5. **Documentation**:
   - Setup instructions
   - API documentation
   - Deployment guide

## 🔑 Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@omnibus.tn | Admin123! |
| Driver | driver1@omnibus.tn | Driver123! |
| Driver | driver2@omnibus.tn | Driver123! |
| Passenger | (Register new account) | (Your password) |

## 📄 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

**Built with ❤️ for Tunisia's Public Transportation System**
"# OmniBus" 
