# 🚀 OmniBus Quick Start Guide

## ⚡ Run in 3 Steps

### Step 1: Configure Database
Edit `src\OmniBus.API\appsettings.Development.json`:
```json
"DefaultConnection": "Host=localhost;Database=OmniBusDb;Username=postgres;Password=YOUR_PASSWORD"
```

### Step 2: Run Backend
```bash
cd c:\Users\Lenovo\Downloads\OmniBus\src\OmniBus.API

# First time only - create database
dotnet ef database update --project ..\OmniBus.Infrastructure\OmniBus.Infrastructure.csproj

# Start backend server
dotnet run
```
✅ Backend running at: **https://localhost:5001**

### Step 3: Run Frontend
```bash
# Open a NEW terminal window
cd c:\Users\Lenovo\Downloads\OmniBus\src\OmniBus.Client

# First time only - install dependencies
npm install

# Start frontend server
npm run dev
```
✅ Frontend running at: **http://localhost:5173**

---

## 🎯 Access Points

- **Web App**: http://localhost:5173
- **API Swagger**: https://localhost:5001/swagger
- **API Base URL**: https://localhost:5001/api

---

## 📋 Common Commands

### Backend Commands
```bash
# Build project
dotnet build

# Run tests
dotnet test

# Create new migration
dotnet ef migrations add MigrationName --project ..\OmniBus.Infrastructure\OmniBus.Infrastructure.csproj

# Update database
dotnet ef database update --project ..\OmniBus.Infrastructure\OmniBus.Infrastructure.csproj
```

### Frontend Commands
```bash
# Development mode
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint
```

---

## ✅ Build Status

**Backend**: ✅ Build succeeded - 0 Errors  
**Frontend**: ✅ Dependencies installed  
**Database**: ⚠️ Needs configuration  

---

For detailed documentation, see [PROJECT_STATUS.md](PROJECT_STATUS.md)
