# Testing Guide - Learning Dashboard System

## ✅ Build Verification - PASSED

The application builds successfully with no errors or warnings.

---

## 🧪 Testing Plan

### Phase 1: Local Testing (30 minutes)

#### Step 1: Database Setup
```powershell
# In Package Manager Console
Add-Migration AddLearningModels
Update-Database
```

**Verify**:
- [ ] Migration created successfully
- [ ] No SQL errors
- [ ] Database tables created:
  - Learnings
  - LearningAssignments

#### Step 2: Run Application
```
Press F5 or Ctrl+Shift+D to start debugging
```

**Verify**:
- [ ] Application starts without errors
- [ ] No runtime exceptions
- [ ] Port: https://localhost:7000 (or configured port)
- [ ] Static files load (Bootstrap CSS, JavaScript)

#### Step 3: Test Authentication
1. Navigate to `https://localhost:7000/Account/Login`
2. Login with test credentials (or create new user)
3. Verify authentication successful

**Verify**:
- [ ] Login page displays
- [ ] Can login with valid credentials
- [ ] Redirects to dashboard after login
- [ ] User name displays in header

---

### Phase 2: Role-Based Access Testing (15 minutes)

#### As Employee User:
1. Login as employee
2. Navigate to `/Employee`
3. Verify dashboard loads

**Verify**:
- [ ] Employee dashboard displays
- [ ] Access to `/Manager` denied (redirect to access denied)
- [ ] Access to `/HR` denied (redirect to access denied)
- [ ] Page title: "My Learning Dashboard"

#### As Manager User:
1. Logout (click logout link)
2. Login as manager
3. Navigate to `/Manager`

**Verify**:
- [ ] Manager dashboard displays
- [ ] Page title: "Team Learning Dashboard"
- [ ] Access to `/HR` denied
- [ ] Can access `/Employee` (but it's their own)

#### As HR User:
1. Logout
2. Login as HR user
3. Navigate to `/HR`

**Verify**:
- [ ] HR dashboard displays
- [ ] Page title: "HR Learning Analytics Dashboard"
- [ ] Can access other dashboards (but they're not their role's primary dashboard)

---

### Phase 3: Dashboard UI Testing (20 minutes)

#### Employee Dashboard
1. Verify page elements:

**Check these elements**:
- [ ] Header with user name displayed
- [ ] 4 summary cards visible:
  - Total Assigned (blue)
  - Completed (green)
  - In Progress (yellow)
  - Not Started (red)
- [ ] "My Learning Assignments" table
- [ ] Table columns: Title, Category, Status, Progress, Dates, Days Left, Action

**If no data shows**:
- [ ] Check browser console (F12) for errors
- [ ] Verify API returned data
- [ ] Ensure database has assignments for this user

**Test Update Feature**:
1. Click "Update" button on any assignment
2. Modal dialog opens
3. Change status or progress slider
4. Click "Save Changes"

**Verify**:
- [ ] Modal opens correctly
- [ ] Can change status dropdown
- [ ] Can move progress slider (0-100%)
- [ ] Progress value updates as you move slider
- [ ] Save button closes modal
- [ ] Page refreshes with updated data

#### Manager Dashboard
1. Verify page elements:

**Check these elements**:
- [ ] Header with user name
- [ ] 6 summary metric cards:
  - Team Members
  - Total Assigned
  - Completed
  - In Progress
  - Not Started
  - Completion Rate
- [ ] Two tabs: "Team Overview" and "All Assignments"

**Team Overview Tab**:
- [ ] Team member cards display (one card per team member)
- [ ] Each card shows:
  - Member name
  - Completion progress bar
  - Breakdown: Total, Completed, In Progress, Not Started
  - Color-coded numbers

**All Assignments Tab**:
- [ ] Table displays all team assignments
- [ ] Columns: Employee, Title, Status, Progress, Dates
- [ ] Progress bars show percentage
- [ ] Data matches Team Overview

#### HR Dashboard
1. Verify page elements:

**Check these elements**:
- [ ] 6 summary metric cards visible
- [ ] Two tabs: "Employee Progress" and "Category Analysis"

**Charts Section**:
- [ ] "Assignment Status Distribution" pie/doughnut chart visible
  - Shows: Completed, In Progress, Not Started
  - Colors: Green, Yellow, Red
  - Legend displays
- [ ] "Category Completion Rate" bar chart visible
  - Shows categories on Y-axis
  - Completion rate on X-axis (0-100%)

**Employee Progress Tab**:
- [ ] Table with employees
- [ ] Columns: Employee, Total Assigned, Completed, Completion Rate, Progress
- [ ] Progress bars color-coded:
  - Green (≥80%), Yellow (50-80%), Red (<50%)
- [ ] Data reflects correct calculations

**Category Analysis Tab**:
- [ ] Table with learning categories
- [ ] Columns: Category, Total Assigned, Completed, Completion Rate, Progress
- [ ] Similar color coding for progress bars

---

### Phase 4: API Testing (15 minutes)

Open Developer Tools (F12) → Network tab to monitor API calls.

#### Test Employee Dashboard API
1. As logged-in employee
2. Navigate to Employee dashboard
3. Check Network tab

**Look for**:
- [ ] `GET /api/dashboard/employee` request
- [ ] Status: 200 OK
- [ ] Response contains:
  - `totalAssignments` (number)
  - `completed` (number)
  - `inProgress` (number)
  - `notStarted` (number)
  - `assignments` (array)

**Sample response structure**:
```json
{
  "totalAssignments": 5,
  "completed": 2,
  "inProgress": 2,
  "notStarted": 1,
  "assignments": [
    {
      "id": 1,
      "title": "...",
      "category": "...",
      "status": "...",
      "assignedDate": "2024-...",
      "dueDate": "2024-...",
      "progressPercentage": 50,
      "completedDate": null,
      "daysUntilDue": 10
    }
  ]
}
```

#### Test Manager Dashboard API
1. As logged-in manager
2. Navigate to Manager dashboard
3. Check Network tab

**Look for**:
- [ ] `GET /api/dashboard/manager` request
- [ ] Status: 200 OK
- [ ] Response contains:
  - `totalTeamMembers` (number)
  - `totalAssignments` (number)
  - `completionRate` (percentage)
  - `teamAssignments` (array of team members)

#### Test HR Dashboard API
1. As logged-in HR user
2. Navigate to HR dashboard
3. Check Network tab

**Look for**:
- [ ] `GET /api/dashboard/hr` request
- [ ] Status: 200 OK
- [ ] Response contains:
  - `totalEmployees` (number)
  - `totalManagers` (number)
  - `overallCompletionRate` (percentage)
  - `completionByCategory` (array)
  - `employeeProgress` (array)

---

### Phase 5: Data Management Testing (10 minutes)

#### Using Postman or cURL

**Create a Learning Assignment**:
```bash
curl -X POST https://localhost:7000/api/assignments \
  -H "Content-Type: application/json" \
  -d '{"userId":5,"learningId":2,"dueDate":"2024-02-28"}'
```

**Verify**:
- [ ] Returns 201 Created
- [ ] Response includes new assignment ID
- [ ] Assignment appears in dashboard

**Update Assignment Status**:
```bash
curl -X PUT https://localhost:7000/api/assignments/1 \
  -H "Content-Type: application/json" \
  -d '{"status":"InProgress","progressPercentage":50}'
```

**Verify**:
- [ ] Returns 204 No Content
- [ ] Dashboard updates with new status
- [ ] Progress bar reflects new percentage

---

### Phase 6: Error Handling Testing (10 minutes)

#### Test 401 Unauthorized
1. Open new browser window (private/incognito)
2. Navigate to `https://localhost:7000/api/dashboard/employee`

**Verify**:
- [ ] Redirects to login page
- [ ] Does not expose API data

#### Test 403 Forbidden
1. Login as Employee
2. Try to POST to `/api/learnings` (create learning)
3. Check response

**Verify**:
- [ ] Returns 403 Forbidden
- [ ] Error message displayed if applicable

#### Test 404 Not Found
1. Navigate to `/api/learnings/99999` (non-existent ID)

**Verify**:
- [ ] Returns 404 Not Found
- [ ] Appropriate error response

---

### Phase 7: Responsive Design Testing (10 minutes)

#### Desktop (1920x1080)
1. Open dashboard
2. Press F12 to open DevTools
3. View at 100% zoom

**Verify**:
- [ ] All elements visible
- [ ] Cards display in rows
- [ ] No horizontal scrolling needed
- [ ] Table fully readable

#### Tablet (768x1024)
1. In DevTools, select iPad view
2. Reload page

**Verify**:
- [ ] Cards stack appropriately
- [ ] 2-column layout adjusts to narrower
- [ ] Table still readable
- [ ] Modals display correctly

#### Mobile (375x667)
1. In DevTools, select iPhone view
2. Reload page

**Verify**:
- [ ] Cards stack vertically
- [ ] Single column layout
- [ ] Table has horizontal scroll (not broken)
- [ ] Buttons clickable
- [ ] Modals display correctly

---

### Phase 8: Browser Console Testing (5 minutes)

1. Open DevTools (F12)
2. Go to Console tab
3. Navigate through dashboards
4. Perform actions (update, click buttons)

**Verify**:
- [ ] No JavaScript errors (red messages)
- [ ] No 404 errors for resources
- [ ] No CORS errors
- [ ] Console is clean (maybe warnings but no errors)

**Common issues to check**:
- [ ] Chart.js library loaded successfully
- [ ] Bootstrap loaded
- [ ] Fetch API calls show correct endpoints
- [ ] No undefined variable references

---

## 📋 Quick Test Checklist

### Before Testing
- [ ] Build successful
- [ ] Database migrated
- [ ] Application running

### During Testing
- [ ] Employee dashboard loads
- [ ] Manager dashboard loads
- [ ] HR dashboard loads
- [ ] Charts render in HR dashboard
- [ ] API calls return 200 OK
- [ ] Update functionality works
- [ ] Role-based access working
- [ ] No console errors
- [ ] Responsive design works

### Success Criteria
- [ ] All dashboards display data
- [ ] API endpoints respond correctly
- [ ] Authorization enforced
- [ ] No JavaScript errors
- [ ] Mobile/tablet responsive
- [ ] All metrics calculate correctly

---

## 🐛 Troubleshooting Common Issues

### Dashboard shows "No learning assignments at this time"
**Solution**: 
1. Check database has assignments for this user
2. Verify assignment UserID matches logged-in user
3. Check API response in Network tab
4. See: DASHBOARD_QUICK_REFERENCE.md Troubleshooting

### API returns 401 Unauthorized
**Solution**:
1. Verify user is logged in
2. Check authentication cookie exists (DevTools → Application → Cookies)
3. Verify role is set correctly in database

### Charts don't render (HR Dashboard)
**Solution**:
1. Check Chart.js loaded (DevTools → Network → look for Chart.js CDN)
2. Verify API returns `completionByCategory` data
3. Check console for JavaScript errors
4. Ensure data array is not empty

### "The resource you are looking for has been removed..."
**Solution**:
1. Verify you're logged in with correct role
2. Check authorization policy matches your role
3. See: SETUP_DEPLOYMENT_CHECKLIST.md Authorization section

### Modal doesn't open when clicking Update
**Solution**:
1. Check console for JavaScript errors
2. Verify Bootstrap loaded
3. Check button has correct onclick handler
4. Clear browser cache and reload

---

## 📊 Expected Data Flow

```
1. User logs in
   ↓
2. Cookie-based authentication created
   ↓
3. User navigates to dashboard
   ↓
4. Dashboard page loads Razor view
   ↓
5. JavaScript calls API endpoint
   ↓
6. API controller checks authorization
   ↓
7. Database query fetches user data
   ↓
8. JSON response sent to browser
   ↓
9. JavaScript renders data in UI
   ↓
10. User sees dashboard with data
```

---

## ⏱️ Performance Benchmarks

Target times (for development):
- Page load: < 2 seconds
- API response: < 500ms
- Chart rendering: < 1 second
- Modal open: < 200ms

If slower, check:
- Network tab (API call time)
- Database query performance
- Browser DevTools Performance tab

---

## 🎓 Test Scenarios by Role

### Employee User Workflow
1. Login as employee
2. View personal dashboard
3. See assigned learnings
4. Update progress on one learning
5. Verify update reflected in dashboard
6. Logout

### Manager User Workflow
1. Login as manager
2. View team dashboard
3. See team metrics
4. View team member cards
5. Switch to "All Assignments" tab
6. View all team assignments
7. Logout

### HR User Workflow
1. Login as HR
2. View HR dashboard
3. See organization metrics
4. Observe charts render
5. View employee progress table
6. View category analysis
7. Logout

---

## ✅ Testing Completion

You've successfully tested the application when:
- All dashboards load with data
- All API endpoints return correct responses
- Role-based access is enforced
- No critical errors in console
- Responsive design works on all breakpoints
- Update functionality works end-to-end

---

## 📞 If You Encounter Issues

1. **Check Documentation**: Review DASHBOARD_QUICK_REFERENCE.md
2. **Review Console**: Press F12 → Console tab
3. **Check Network**: Press F12 → Network tab
4. **Verify Database**: Ensure migrations applied
5. **Check Auth**: Verify user role in database
6. **Read Troubleshooting**: See SETUP_DEPLOYMENT_CHECKLIST.md

---

**Good luck with testing! 🚀**

For detailed information, see:
- DOCUMENTATION_INDEX.md - All documentation
- API_TESTING_GUIDE.md - API testing examples
- DASHBOARD_QUICK_REFERENCE.md - Feature reference
