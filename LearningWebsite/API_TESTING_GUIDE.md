# API Testing Guide with cURL Examples

## Prerequisites
- Application running on `https://localhost:7000`
- User authenticated and logged in (cookies will be sent automatically)
- Headers: `Content-Type: application/json`

## Dashboard Endpoints

### Get Employee Dashboard
```bash
curl -X GET https://localhost:7000/api/dashboard/employee \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (200 OK):**
```json
{
  "totalAssignments": 5,
  "completed": 2,
  "inProgress": 2,
  "notStarted": 1,
  "assignments": [
    {
      "id": 1,
      "title": "C# Fundamentals",
      "category": "Technical",
      "status": "InProgress",
      "assignedDate": "2024-01-15T00:00:00",
      "dueDate": "2024-02-15T00:00:00",
      "progressPercentage": 50,
      "completedDate": null,
      "daysUntilDue": 10
    }
  ]
}
```

### Get Manager Dashboard
```bash
curl -X GET https://localhost:7000/api/dashboard/manager \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (200 OK):**
```json
{
  "totalTeamMembers": 5,
  "totalAssignments": 20,
  "completedAssignments": 8,
  "inProgressAssignments": 7,
  "notStartedAssignments": 5,
  "completionRate": 40.0,
  "teamAssignments": [
    {
      "userName": "employee1",
      "totalAssigned": 4,
      "completed": 2,
      "inProgress": 1,
      "notStarted": 1,
      "assignments": [
        {
          "id": 1,
          "title": "C# Fundamentals",
          "status": "Completed",
          "assignedDate": "2024-01-15T00:00:00",
          "dueDate": "2024-02-15T00:00:00",
          "progressPercentage": 100
        }
      ]
    }
  ]
}
```

### Get HR Dashboard
```bash
curl -X GET https://localhost:7000/api/dashboard/hr \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (200 OK):**
```json
{
  "totalEmployees": 5,
  "totalManagers": 2,
  "totalAssignments": 50,
  "completedAssignments": 25,
  "inProgressAssignments": 15,
  "notStartedAssignments": 10,
  "overallCompletionRate": 50.0,
  "completionByCategory": [
    {
      "category": "Technical",
      "total": 20,
      "completed": 12,
      "completionRate": 60.0
    },
    {
      "category": "Soft Skills",
      "total": 15,
      "completed": 8,
      "completionRate": 53.33
    }
  ],
  "employeeProgress": [
    {
      "userName": "employee1",
      "totalAssignments": 10,
      "completed": 7,
      "completionRate": 70.0
    }
  ]
}
```

## Learning Endpoints

### Get All Learnings
```bash
curl -X GET https://localhost:7000/api/learnings \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (200 OK):**
```json
[
  {
    "id": 1,
    "title": "C# Fundamentals",
    "description": "Learn the basics of C# programming language",
    "category": "Technical",
    "durationInHours": 30,
    "assignments": []
  },
  {
    "id": 2,
    "title": "Leadership Skills",
    "description": "Develop effective leadership and management skills",
    "category": "Soft Skills",
    "durationInHours": 20,
    "assignments": []
  }
]
```

### Get Specific Learning
```bash
curl -X GET https://localhost:7000/api/learnings/1 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (200 OK):**
```json
{
  "id": 1,
  "title": "C# Fundamentals",
  "description": "Learn the basics of C# programming language",
  "category": "Technical",
  "durationInHours": 30,
  "assignments": []
}
```

### Create Learning (HR Only)
```bash
curl -X POST https://localhost:7000/api/learnings \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Docker Fundamentals",
    "description": "Learn containerization with Docker",
    "category": "Technical",
    "durationInHours": 25
  }'
```

**Expected Response (201 Created):**
```json
{
  "id": 9,
  "title": "Docker Fundamentals",
  "description": "Learn containerization with Docker",
  "category": "Technical",
  "durationInHours": 25,
  "assignments": []
}
```

### Update Learning (HR Only)
```bash
curl -X PUT https://localhost:7000/api/learnings/1 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "C# Advanced Concepts",
    "description": "Learn advanced C# programming language concepts",
    "category": "Technical",
    "durationInHours": 40,
    "assignments": []
  }'
```

**Expected Response (204 No Content)**

### Delete Learning (HR Only)
```bash
curl -X DELETE https://localhost:7000/api/learnings/1 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (204 No Content)**

## Assignment Endpoints

### Create Assignment (Manager/HR Only)
```bash
curl -X POST https://localhost:7000/api/assignments \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 5,
    "learningId": 2,
    "dueDate": "2024-02-28T00:00:00"
  }'
```

**Expected Response (201 Created):**
```json
{
  "id": 15,
  "userId": 5,
  "learningId": 2,
  "assignedDate": "2024-01-20T14:30:00",
  "dueDate": "2024-02-28T00:00:00",
  "status": "NotStarted",
  "progressPercentage": 0,
  "completedDate": null,
  "user": null,
  "learning": null
}
```

### Get Assignment
```bash
curl -X GET https://localhost:7000/api/assignments/15 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (200 OK):**
```json
{
  "id": 15,
  "userId": 5,
  "learningId": 2,
  "assignedDate": "2024-01-20T14:30:00",
  "dueDate": "2024-02-28T00:00:00",
  "status": "NotStarted",
  "progressPercentage": 0,
  "completedDate": null,
  "user": {
    "id": 5,
    "userName": "employee1",
    "passwordHash": "...",
    "role": "Employee",
    "assignments": []
  },
  "learning": {
    "id": 2,
    "title": "Leadership Skills",
    "description": "...",
    "category": "Soft Skills",
    "durationInHours": 20,
    "assignments": []
  }
}
```

### Update Assignment Status & Progress
```bash
curl -X PUT https://localhost:7000/api/assignments/15 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "InProgress",
    "progressPercentage": 50
  }'
```

**Expected Response (204 No Content)**

### Update to Completed
```bash
curl -X PUT https://localhost:7000/api/assignments/15 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Completed",
    "progressPercentage": 100
  }'
```

**Expected Response (204 No Content)**

### Delete Assignment (Manager/HR Only)
```bash
curl -X DELETE https://localhost:7000/api/assignments/15 \
  -H "Accept: application/json" \
  -H "Content-Type: application/json"
```

**Expected Response (204 No Content)**

## Error Scenarios

### 401 Unauthorized (Not Logged In)
```bash
curl -X GET https://localhost:7000/api/dashboard/employee
```

**Response (401 Unauthorized):**
```json
{
  "error": "Unauthorized"
}
```

### 403 Forbidden (Wrong Role)
```bash
# Employee trying to create learning (requires HR)
curl -X POST https://localhost:7000/api/learnings \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test",
    "description": "Test",
    "category": "Technical",
    "durationInHours": 10
  }'
```

**Response (403 Forbidden):**
```json
{
  "error": "Forbidden"
}
```

### 400 Bad Request (Invalid Data)
```bash
curl -X POST https://localhost:7000/api/assignments \
  -H "Accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 999,
    "learningId": 999,
    "dueDate": "2024-02-28T00:00:00"
  }'
```

**Response (400 Bad Request):**
```json
{
  "error": "User not found"
}
```

### 404 Not Found
```bash
curl -X GET https://localhost:7000/api/learnings/999
```

**Response (404 Not Found):**
```json
{
  "error": "Not found"
}
```

## Using Postman

1. **Create Collection**: "Learning Dashboard API"
2. **Add Environment Variable**: `base_url = https://localhost:7000`
3. **Create Requests** using `{{base_url}}/api/...` in the URL
4. **Enable Cookies**: Postman automatically saves authentication cookies
5. **Test Each Role**: Login as Employee, Manager, and HR to test role-based access

## Using PowerShell

```powershell
$headers = @{
    "Content-Type" = "application/json"
    "Accept" = "application/json"
}

# Get Employee Dashboard
$response = Invoke-WebRequest -Uri "https://localhost:7000/api/dashboard/employee" `
    -Method Get `
    -Headers $headers `
    -UseDefaultCredentials `
    -SkipHttpErrorCheck

$response.Content | ConvertFrom-Json | Format-Table

# Create Assignment
$body = @{
    userId = 5
    learningId = 2
    dueDate = "2024-02-28T00:00:00"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "https://localhost:7000/api/assignments" `
    -Method Post `
    -Headers $headers `
    -Body $body `
    -UseDefaultCredentials `
    -SkipHttpErrorCheck
```

## Batch Testing Script

Save as `test-api.sh`:
```bash
#!/bin/bash

BASE_URL="https://localhost:7000"

# Test Employee Dashboard
echo "=== Testing Employee Dashboard ==="
curl -s "$BASE_URL/api/dashboard/employee" | jq '.'

# Test Manager Dashboard
echo -e "\n=== Testing Manager Dashboard ==="
curl -s "$BASE_URL/api/dashboard/manager" | jq '.'

# Test HR Dashboard
echo -e "\n=== Testing HR Dashboard ==="
curl -s "$BASE_URL/api/dashboard/hr" | jq '.'

# Test Get Learnings
echo -e "\n=== Testing Get Learnings ==="
curl -s "$BASE_URL/api/learnings" | jq '.'
```

Run with: `bash test-api.sh`

## Debugging Tips

1. **Enable SSL Certificate Bypass** (Development only):
   ```bash
   curl -k https://localhost:7000/api/dashboard/employee
   ```

2. **Verbose Output**:
   ```bash
   curl -v https://localhost:7000/api/dashboard/employee
   ```

3. **View Response Headers**:
   ```bash
   curl -i https://localhost:7000/api/dashboard/employee
   ```

4. **Pretty Print JSON** (requires `jq`):
   ```bash
   curl https://localhost:7000/api/dashboard/employee | jq '.'
   ```

5. **Save Response to File**:
   ```bash
   curl https://localhost:7000/api/dashboard/employee > response.json
   ```
