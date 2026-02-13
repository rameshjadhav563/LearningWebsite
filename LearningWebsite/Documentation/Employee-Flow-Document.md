# Employee Flow - Functional Documentation
## Learning Management System

---

## 1. Overview
The Employee role represents end-users who are assigned learning modules by their managers or HR. Employees can view their assignments, track their progress, update their profile, and manage their learning activities.

---

## 2. Access Control
- **Policy**: EmployeeOnly
- **Authorization**: Required for all Employee controller actions
- **Role**: Employee
- **Scope**: Can only access their own data and assignments

---

## 3. Functional Flows

### 3.1 Employee Dashboard (Index)
**Purpose**: Provides employee with an overview of their learning journey

**Endpoint**: `/Employee/Index`

#### Flow Steps:
1. Employee logs into the system
2. System authenticates using claims-based authentication
3. Dashboard loads with:
   - **Welcome Message**: Greeting with employee name
   - **Quick Stats**:
     - Total Assignments
     - Pending Assignments
     - Completed Assignments
     - Completion Rate
   - **Action Cards**:
     - View My Profile
     - View My Assignments
     - Track Progress
   - **Recent Activity**:
     - Recently assigned learnings
     - Recently completed learnings
     - Upcoming due dates

#### Business Rules:
- Dashboard is personalized to logged-in employee
- Only employee's own data is visible
- Dashboard access is logged for audit purposes
- Real-time calculation of metrics

---

### 3.2 Profile Management

#### 3.2.1 View Profile
**Purpose**: Display employee's complete profile and assignment overview

**Endpoint**: `/Employee/MyProfile` (GET)

**Flow Steps**:
1. Employee clicks "My Profile"
2. System retrieves employee data including:
   - Personal information
   - Manager information
   - All learning assignments with details
3. Displays:
   - **Personal Information**:
     - Username (read-only)
     - Full Name
     - Email
     - Role (read-only)
   - **Manager Information**:
     - Manager Name
     - Manager Email (if available)
   - **Learning Summary**:
     - Total Assignments
     - Completed Count
     - In-Progress Count
     - Not Started Count
     - Overall Completion Rate
   - **Assignment List**:
     - All assigned learnings with:
       - Title
       - Category
       - Status
       - Progress %
       - Due Date
       - Completion Date (if completed)

#### Business Rules:
- Profile is read-only except editable fields
- Username and Role cannot be changed by employee
- Manager assignment is managed by HR only
- All timestamps display in user's local time

#### Data Loaded:
```csharp
Include(u => u.Manager)
Include(u => u.Assignments).ThenInclude(a => a.Learning)
```

---

#### 3.2.2 Edit Profile
**Purpose**: Allow employees to update their personal information and password

**Endpoint**: `/Employee/EditProfile` (GET & POST)

**GET Flow Steps**:
1. Employee clicks "Edit Profile" button
2. System loads current profile data
3. Displays editable form with:
   - Full Name (editable)
   - Email (editable)
   - Current Password (for verification)
   - New Password (optional)
   - Confirm Password (optional)

**POST Flow Steps**:
1. Employee submits form with changes
2. System validates:
   - Required fields (Full Name, Email)
   - Email format
   - Password requirements (if changing password)
   - Password confirmation match
3. If password change requested:
   - Validate password length (minimum 6 characters)
   - Verify new password matches confirmation
   - Hash new password using Identity Password Hasher
   - Update password hash
4. Update allowed fields:
   - Full Name
   - Email
5. Save changes to database
6. Log profile update action
7. Display success message: "Profile updated successfully!"
8. Redirect to MyProfile page

#### Editable Fields:
- ✅ Full Name
- ✅ Email
- ✅ Password (optional change)
- ❌ Username (read-only)
- ❌ Role (read-only)
- ❌ Manager (read-only)

#### Password Change Validation:
- Minimum length: 6 characters
- Must match confirmation field
- Cannot be empty string (if provided)
- Hashed using `IPasswordHasher<ApplicationUser>`

#### Business Rules:
1. Employee can only edit their own profile
2. Username cannot be changed (permanent identifier)
3. Role is assigned by HR and cannot be self-modified
4. Manager assignment is controlled by HR
5. Password change is optional during profile edit
6. All password changes are logged for security audit
7. Old password hash is replaced (no password history)

#### Security Considerations:
- Current user identification via Claims: `ClaimTypes.Name`
- No user ID in URL (prevents tampering)
- Password is hashed before storage
- ModelState validation prevents injection attacks

---

### 3.3 Learning Assignment Management

#### 3.3.1 View All Assignments
**Purpose**: Display all learning assignments assigned to the employee

**Flow Steps**:
1. Employee navigates to "My Assignments"
2. System retrieves all assignments where `UserId = Employee.Id`
3. Displays assignments in table format:
   - Learning Title
   - Category
   - Description
   - Duration (hours)
   - Assigned Date
   - Due Date
   - Status
   - Progress Percentage
   - Actions (Start, Continue, View)

#### Filtering & Sorting:
- **Filter by Status**:
  - All
  - Not Started
  - In Progress
  - Completed
- **Filter by Category**:
  - Technical
  - Soft Skills
  - Compliance
  - Leadership
  - Others
- **Sort by**:
  - Due Date (ascending/descending)
  - Assigned Date
  - Title (alphabetical)
  - Progress

#### Status Indicators:
| Status | Color | Icon | Description |
|--------|-------|------|-------------|
| NotStarted | Gray | ⏸ | Not yet begun |
| InProgress | Blue | ▶ | Currently learning |
| Completed | Green | ✓ | Finished |
| Overdue | Red | ⚠ | Past due date, not completed |

---

#### 3.3.2 Start Learning
**Purpose**: Begin a learning module

**Endpoint**: `/Employee/StartLearning/{id}` (POST)

**Flow Steps**:
1. Employee clicks "Start" on an assignment
2. System validates:
   - Assignment exists
   - Assignment belongs to current employee
   - Current status is "NotStarted"
3. Updates assignment:
   - `Status = "InProgress"`
   - `ProgressPercentage = 0` (initial)
4. Redirects to learning content or external URL
5. Logs start action

**Business Rules**:
- Can only start assignments with status "NotStarted"
- Cannot restart completed assignments (use "Review" instead)
- Start action is logged with timestamp

---

#### 3.3.3 Update Progress
**Purpose**: Track progress through a learning module

**Endpoint**: `/Employee/UpdateProgress/{id}` (POST)

**Flow Steps**:
1. Employee interacts with learning content
2. System periodically or on milestone saves progress
3. Validates:
   - Assignment exists
   - Belongs to current employee
   - Not already completed
4. Updates:
   - `ProgressPercentage` (0-100)
   - `Status`:
     - 0% → "NotStarted"
     - 1-99% → "InProgress"
     - 100% → "Completed"
5. If progress = 100%:
   - Set `Status = "Completed"`
   - Set `CompletedDate = DateTime.Now`
6. Save to database
7. Return updated progress

**Business Rules**:
- Progress must be between 0-100
- Status automatically updates based on progress
- Completion date set only when progress = 100%
- Cannot decrease progress once completed
- Progress updates are logged

---

#### 3.3.4 Mark as Complete
**Purpose**: Manually mark a learning assignment as completed

**Endpoint**: `/Employee/CompleteAssignment/{id}` (POST)

**Flow Steps**:
1. Employee clicks "Mark as Complete" button
2. System validates:
   - Assignment exists
   - Belongs to current employee
   - Status is not already "Completed"
3. Optional: Show completion confirmation modal
4. Updates assignment:
   - `Status = "Completed"`
   - `ProgressPercentage = 100`
   - `CompletedDate = DateTime.Now`
5. Log completion action
6. Display success message
7. Optional: Trigger completion notification to manager
8. Redirect to assignments page

**Business Rules**:
- Can only complete own assignments
- Completion sets progress to 100%
- Completion date uses server timestamp
- Cannot un-complete (only HR/Manager can reset)

---

#### 3.3.5 View Learning Details
**Purpose**: View comprehensive details about a specific learning module

**Endpoint**: `/Employee/LearningDetail/{id}` (GET)

**Flow Steps**:
1. Employee clicks on learning title
2. System retrieves learning details
3. Displays:
   - **Learning Information**:
     - Title
     - Description (full)
     - Category
     - Duration in hours
     - Prerequisites (if any)
   - **Assignment Information**:
     - Assigned Date
     - Due Date
     - Days Remaining
     - Current Status
     - Current Progress
   - **Learning Content**:
     - Content URL (if external)
     - Embedded content (if internal)
     - Resources/Downloads
   - **Actions**:
     - Start/Continue button
     - Mark as Complete button
     - Download Resources

---

### 3.4 Progress Tracking

#### 3.4.1 Progress Dashboard
**Purpose**: Visual representation of learning progress

**Metrics Displayed**:
- **Overall Progress**:
  - Completion rate (pie chart)
  - Total vs Completed assignments
- **Category Breakdown**:
  - Progress by learning category
  - Bar chart visualization
- **Timeline**:
  - Completion history over time
  - Line chart showing trend
- **Status Distribution**:
  - Count by status (NotStarted/InProgress/Completed)
  - Donut chart

#### 3.4.2 Upcoming Deadlines
**Purpose**: Track due dates and avoid overdue assignments

**Display**:
- Assignments due within 7 days (highlighted)
- Assignments due within 30 days
- Overdue assignments (red alert)

**Actions**:
- Sort by due date (nearest first)
- Filter by overdue only
- Set personal reminders

---

### 3.5 Notifications (Future)

#### 3.5.1 Assignment Notifications
- New assignment email
- Due date reminders (7 days, 3 days, 1 day)
- Overdue alerts
- Completion acknowledgment

#### 3.5.2 In-App Notifications
- Badge counter for unread notifications
- Notification center
- Real-time push notifications

---

## 4. Data Access Scope

### 4.1 What Employee CAN Access:
✅ Own profile information  
✅ Own learning assignments  
✅ Own progress data  
✅ Learning content assigned to them  
✅ Manager information (read-only)  
✅ Own completion history  

### 4.2 What Employee CANNOT Access:
❌ Other employees' data  
❌ Manager functionalities  
❌ HR functionalities  
❌ System-wide reports  
❌ Other users' profiles  
❌ Unassigned learning content (browse-only if allowed)  
❌ Manager assignment changes  

---

## 5. Security & Authorization

### 5.1 Authentication
- Claims-based authentication: `ClaimTypes.Name`
- Session-based login required
- Token validation on each request

### 5.2 Authorization
**Policy**: EmployeeOnly
```csharp
[Authorize(Policy = "EmployeeOnly")]
```

### 5.3 Data Filtering
All queries filtered by current user:
```csharp
.FirstOrDefault(u => u.UserName == userName)
.Where(a => a.UserId == employee.Id)
```

### 5.4 Validation Checks
1. User identity verification
2. Assignment ownership verification
3. Status transition validation
4. Progress value validation (0-100)

### 5.5 Security Responses
- Unauthorized user → 401 Unauthorized
- Access to other user's data → 403 Forbidden
- Invalid assignment ID → 404 Not Found

---

## 6. Key Features Summary

| Feature | Access | Description |
|---------|--------|-------------|
| View Dashboard | Self Only | Personal learning overview |
| View Profile | Self Only | Personal information and manager |
| Edit Profile | Self Only | Update name, email, password |
| View Assignments | Self Only | All assigned learnings |
| Start Learning | Self Only | Begin a learning module |
| Update Progress | Self Only | Track progress through content |
| Complete Assignment | Self Only | Mark learning as done |
| View Learning Details | Self Only | Detailed learning information |
| Track Progress | Self Only | Visual progress dashboard |
| View Deadlines | Self Only | Upcoming and overdue items |

---

## 7. Business Rules Summary

1. Employee can only access their own data
2. Profile edits limited to Full Name, Email, and Password
3. Username and Role are immutable by employee
4. Manager assignment controlled by HR only
5. Password changes require confirmation match
6. Minimum password length: 6 characters
7. Progress updates must be 0-100%
8. Completion automatically sets progress to 100%
9. Completion date uses server timestamp
10. Status transitions:
    - NotStarted → InProgress (on start)
    - InProgress → Completed (on 100% or manual complete)
11. Cannot un-complete assignments (requires HR/Manager)
12. All actions are logged for audit
13. Overdue status = DueDate < Today AND Status != Completed

---

## 8. Integration Points

- **Authentication**: Claims-based Identity framework
- **Database**: AppDbContext with EF Core
- **Password Hashing**: IPasswordHasher for secure passwords
- **Logging**: ILogger for audit trail
- **Authorization**: Policy-based (EmployeeOnly)
- **Relationships**: Employee → Manager (many-to-one)

---

## 9. Error Handling

### 9.1 Validation Errors
- Missing required fields → Display field-specific errors
- Invalid email format → Email validation error
- Password mismatch → "Passwords do not match"
- Short password → "Password must be at least 6 characters"

### 9.2 Business Logic Errors
- Invalid assignment ID → 404 Not Found
- Access to other user's assignment → 403 Forbidden
- Invalid progress value → Validation error
- Duplicate completion → Silently handled

### 9.3 Database Errors
- Save failure → Log and display generic error
- Connection issues → "Unable to save changes"
- Concurrent updates → Reload and retry

---

## 10. User Experience Flow

### 10.1 Typical Employee Workflow
```
Login 
  → Dashboard (view stats) 
  → My Assignments 
  → Filter by "Not Started" 
  → Select a learning 
  → Start learning 
  → Update progress periodically 
  → Complete learning 
  → View next assignment
```

### 10.2 Profile Update Workflow
```
My Profile 
  → Edit Profile 
  → Update information 
  → Change password (optional) 
  → Save 
  → Confirmation message
```

### 10.3 Learning Completion Workflow
```
My Assignments 
  → Click on learning 
  → View details 
  → Start learning 
  → Content page/external link 
  → Update progress 
  → Mark as complete 
  → Return to assignments
```

---

## 11. Status Lifecycle

```
[New Assignment Created]
        ↓
    NotStarted (Progress: 0%)
        ↓
   [Employee clicks "Start"]
        ↓
    InProgress (Progress: 1-99%)
        ↓
   [Progress reaches 100% OR Manual Complete]
        ↓
    Completed (Progress: 100%, CompletedDate set)
```

### Status Transitions:
- **NotStarted** → **InProgress**: Employee starts learning
- **InProgress** → **Completed**: Progress reaches 100% or manual complete
- **Completed** → Cannot revert (requires HR/Manager intervention)

---

## 12. Progress Calculation

### Automatic Status Update:
```csharp
if (progressPercentage == 0)
    status = "NotStarted";
else if (progressPercentage < 100)
    status = "InProgress";
else if (progressPercentage == 100)
{
    status = "Completed";
    completedDate = DateTime.Now;
}
```

### Completion Rate:
```csharp
CompletionRate = (CompletedCount / TotalAssignments) × 100
```

---

## 13. Audit & Logging

All employee actions are logged:
- Login/logout events
- Profile view and edit actions
- Assignment view actions
- Learning start actions
- Progress updates
- Completion actions

**Log Format**:
```
Timestamp | Employee Username | Action | Assignment/Learning | Details
```

---

## 14. Performance Considerations

- Profile queries use `.Include()` for related data
- Assignment queries filtered by UserId (indexed)
- Progress updates are incremental
- Dashboard metrics calculated on-the-fly
- Caching for frequently accessed data (learning content)

---

## 15. Future Enhancements

1. **Gamification**: Points, badges, leaderboards
2. **Social Learning**: Peer discussions, comments
3. **Learning Paths**: Sequential learning modules
4. **Certificates**: Digital certificates on completion
5. **Mobile App**: Native iOS/Android apps
6. **Offline Access**: Download content for offline learning
7. **Video Streaming**: Embedded video content
8. **Quizzes & Assessments**: Test knowledge
9. **Peer Review**: Feedback from colleagues
10. **Learning Recommendations**: AI-suggested learnings
11. **Progress Sync**: Cross-device progress synchronization
12. **Bookmarks**: Save position in learning content
13. **Notes**: Take notes within learning modules
14. **Export Progress**: PDF report of completed learnings

---

## 16. Accessibility

- Keyboard navigation support
- Screen reader compatible
- High contrast mode
- Font size adjustment
- WCAG 2.1 Level AA compliance

---

## 17. Mobile Responsiveness

- Responsive design for all screens
- Touch-friendly interface
- Mobile-optimized navigation
- Progressive Web App (PWA) support

---

**Document Version**: 1.0  
**Last Updated**: 2025  
**System**: Learning Management System - .NET 8 Razor Pages  
**Role**: Employee
