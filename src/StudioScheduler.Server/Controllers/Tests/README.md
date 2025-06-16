# API Testing with REST Client

This directory contains HTTP test files for testing the Studio Scheduler API endpoints using the VS Code REST Client extension.

## Prerequisites

1. **Install REST Client Extension**: Install the "REST Client" extension in VS Code
2. **Start the Server**: Run the StudioScheduler.Server project (`dotnet run` or F5)
3. **Check Ports**: The tests use ports 7224 (HTTPS) and 5224 (HTTP) - update if different

## Test Files

### 🚀 Quick Start: `AllEndpoints.http`
**Start here!** This file provides a comprehensive test suite that verifies:
- Entity Framework repositories are working
- SalsaMe Dance Studio data is loaded from database
- All major endpoints respond correctly

**How to use:**
1. Open `AllEndpoints.http`
2. Click "Send Request" above each `###` section
3. Verify responses match the expected results listed at the bottom

### 📚 Detailed Tests

#### `ClassesController.http`
Tests for dance class management:
- Get all classes (should show 27+ SalsaMe classes)
- Get specific class by ID
- Create new class
- Update existing class
- Delete class
- Error handling

#### `LocationsController.http`
Tests for location management:
- Get all locations (should show SalsaMe Dance Studio)
- Get specific location
- Get location rooms
- Get location schedules
- CRUD operations

## Expected Mock Data

If everything is working correctly, you should see:

### ✅ Classes Endpoint (`/api/classes`)
```json
[
  {
    "name": "SALSA LADIES STYLING",
    "level": "P3",
    "capacity": 15
  },
  {
    "name": "BACHATA",
    "level": "P1",
    "capacity": 20
  }
  // ... 25+ more classes
]
```

### ✅ Locations Endpoint (`/api/locations`)
```json
[
  {
    "name": "SalsaMe Dance Studio",
    "address": "Nadbystrzycka 25 str., 20-618 Lublin",
    "isActive": true
  }
]
```

### ✅ Location Rooms (`/api/locations/{id}/rooms`)
```json
[
  {
    "name": "Studio A",
    "capacity": 30
  },
  {
    "name": "Studio B", 
    "capacity": 20
  }
]
```

### ✅ Location Schedules (`/api/locations/{id}/schedules`)
Should return 32+ schedule entries with classes like:
- Monday: SALSA LADIES STYLING (16:45), SALSA KUBAŃSKA (17:40), etc.
- Tuesday: HIGH HEELS SEXY DANCE (16:45), BACHATA (17:40), etc.
- And so on through Sunday...

## Troubleshooting

### ❌ Empty Arrays or 404 Errors
- Check that Entity Framework repositories are registered in `Program.cs`
- Verify the Infrastructure project reference is added
- Make sure the server is running

### ❌ Connection Refused
- Check if the server is running on the correct ports
- Try HTTP URLs instead of HTTPS
- Verify launchSettings.json for correct port configuration

### ❌ Service Not Found Errors
- Check that services are registered in DependencyInjection
- Verify all project references are in place
- Check for compilation errors

## How to Use REST Client

1. **Send Single Request**: Click "Send Request" above any request
2. **Send All**: Use Ctrl+Alt+R (Windows/Linux) or Cmd+Alt+R (Mac)
3. **View Response**: Response appears in a new tab
4. **Variables**: The `@baseUrl` variable at the top can be modified for different environments

## Adding New Tests

When adding new endpoints:

1. Add the endpoint test to the appropriate controller file
2. Include expected response examples
3. Add basic CRUD operations (GET, POST, PUT, DELETE)
4. Include error cases (404, validation errors)
5. Update `AllEndpoints.http` with key tests

## Expected Response Codes

- `200 OK`: Successful GET, PUT operations
- `201 Created`: Successful POST operations  
- `204 No Content`: Successful DELETE operations
- `404 Not Found`: Resource not found
- `400 Bad Request`: Validation errors
