# Quick Test Guide - Run This Now!

## 🚀 Steps to Test Your API (Copy & Paste)

### 1. Open Terminal in VS Code
- Press `Ctrl+Shift+`` (backtick) or go to Terminal → New Terminal

### 2. Build the Solution
```bash
dotnet build
```

### 3. Start the Server
```bash
dotnet run --project src/StudioScheduler.Server
```

### 4. Wait for Server to Start
Look for this message:
```
Now listening on: https://localhost:7224
```

### 5. Test the API
- Open `AllEndpoints.http` file
- Click "Send Request" above the first test (DEBUG endpoint)
- Then click "Send Request" above the classes test

## ✅ Expected Results

### Debug Endpoint Should Return:
```json
{
  "classesCount": 27,
  "locationsCount": 1,
  "filePaths": ["FOUND: C:\\...\\classes.json"]
}
```

### Classes Endpoint Should Return:
```json
[
  {
    "id": "c1a2b3c4-1234-5678-9abc-def012345601",
    "name": "SALSA LADIES STYLING",
    "capacity": 15,
    "isActive": true
  }
  // ... more classes
]
```

## 🐛 If It Doesn't Work

Check the console output for error messages like:
- "Loaded X dance classes from mock data"
- "Error loading mock data: ..."

Share any error messages and I can fix them immediately!

## 🎯 What This Tests

✅ Mock repositories are registered  
✅ JSON files are found and loaded  
✅ API endpoints are working  
✅ SalsaMe dance studio data is available  

**This should take less than 2 minutes to test!**
