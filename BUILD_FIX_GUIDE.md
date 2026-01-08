# Quick Fix Script for Build Errors

## Run these commands to fix the remaining compilation errors:

```bash
cd c:\Users\Lenovo\Downloads\OmniBus\src\OmniBus.Infrastructure

# Add missing using statements
powershell -Command "(Get-Content Services/MultiPaymentGatewayService.cs) -replace 'using Microsoft.Extensions.Configuration;', 'using System.Net.Http.Json;`nusing Microsoft.Extensions.Configuration;' | Set-Content Services/MultiPaymentGatewayService.cs"

powershell -Command "(Get-Content Services/TunisieSmsService.cs) -replace 'using Microsoft.Extensions.Configuration;', 'using System.Net.Http.Json;`nusing Microsoft.Extensions.Configuration;' | Set-Content Services/TunisieSmsService.cs"

# Restore NuGet packages
cd ../OmniBus.API
dotnet restore

# Build project
dotnet build
```

## If still encountering errors:

1. **Redis Package**: Ensure StackExchange.Redis is installed
   ```bash
   dotnet add package StackExchange.Redis --version 2.7.20
   ```

2. **HttpClient Extensions**: Ensure Microsoft.Extensions.Http is installed
   ```bash
   dotnet add package Microsoft.AspNetCore.Http.Extensions
   ```

3. **Clean and Rebuild**:
   ```bash
   dotnet clean
   dotnet build --no-incremental
   ```

## Verification:
```bash
dotnet build 2>&1 | findstr /C:"Build succeeded"
```

Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
