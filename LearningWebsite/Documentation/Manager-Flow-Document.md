# Manager Flow - Functional Documentation
## Learning Management System

---

## 1. Overview
The Manager role is responsible for managing their team members' learning activities, assigning learning content, tracking team progress, and monitoring completion rates within their direct reports.

---

## 2. Access Control
- **Policy**: ManagerOnly
- **Authorization**: Required for all Manager controller actions
- **Role**: Manager
- **Scope**: Can only access their assigned team members

---

## 3. Functional Flows

### 3.1 Manager Dashboard (Index)
**Purpose**: Provides overview of manager's team and their learning activities

**Endpoint**: `/Manager/Index`

#### Flow Steps:
1. Manager logs into the system
2. System identifies manager using claims-based authentication
3. Retrieves manager profile from database
4. Loads all team members where:
   - `ManagerId` = Current Manager's ID
   - `Role` = "Employee"
5. Displays:
   - **Manager Information**:
     - Full Name
     - Username
     - Email
   - **Team Metrics**:
     - Total Team Member Count
     - List of all team members
   - **Quick Actions**:
     - View team member details
     - Assign learnings
     - Track progress

#### Business Rules:
- Manager can only see employees directly assigned to them
- Dashboard logs manager access for audit purposes
- Team members are sorted alphabetically by username
- Only employees with `Role = "Employee"` are shown

#### Security:
- Claims-based authentication validates manager identity
- Database query filters by ManagerId to enforce access control

---

### 3.2 Team Member Management

#### 3.2.1 View Team Member Details
**Purpose**: View individual team member's learning assignments and progress

**Endpoint**: `/Manager/TeamMemberDetail/{id}` (GET)

**Flow Steps**:
1. Manager clicks on team member from dashboard
2. System validates:
   - Team member exists
   - Team member's ManagerId matches current manager
3. Retrieves team member with:
   - Personal information (Username, Full Name, Email)
   - All learning assignments
   - Learning details for each assignment
4. Displays:
   - **Team Member Profile**:
     - Username
     - Full Name
     - Email
     - Manager (self)
   - **Assignments Table**:
     - Learning Title
     - Category
     - Assigned Date
     - Due Date
     - Status (NotStarted/InProgress/Completed)
     - Progress Percentage
     - Completed Date (if applicable)
   - **Available Actions**:
     - Assign new learning
     - Assign multiple learnings
     - View learning details

#### Business Rules:
- Manager can only view team members assigned to them
- Access to other managers' team members returns 403 Forbidden
- All assignments are loaded with complete learning details
- Status color coding: 
  - NotStarted → Gray
  - InProgress → Blue
  - Completed → Green

#### Security:
- Double validation: User exists AND ManagerId matches
- If validation fails → 403 Forbidden response
- Prevents horizontal privilege escalation

---

### 3.3 Learning Assignment Flows

#### 3.3.1 Assign Single Learning
**Purpose**: Assign one learning module to a team member

**Endpoint**: `/Manager/AssignLearning` (POST)

**Flow Steps**:
1. Manager views team member detail page
2. Selects a learning from dropdown
3. Clicks "Assign Learning"
4. System validates:
   - Manager identity
   - Team member belongs to manager
   - Learning exists
   - Assignment doesn't already exist
5. If validation passes:
   - Creates new LearningAssignment record
   - Sets default values:
     - `Status = "NotStarted"`
     - `ProgressPercentage = 0`
     - `AssignedDate = Current DateTime`
     - `DueDate = Current DateTime + 30 days`
6. Saves to database
7. Logs assignment action
8. Redirects to team member detail page

**Business Rules**:
- Default due date: 30 days from assignment
- Cannot assign duplicate learning to same user
- If learning already assigned, silently skip (no error)
- Initial progress is always 0%
- Initial status is always "NotStarted"

**Validation**:
- Team member must belong to current manager
- Learning must exist in system
- Manager must be authenticated

---

#### 3.3.2 Assign Multiple Learnings (Bulk Assignment)
**Purpose**: Assign multiple learning modules to a team member at once

**Endpoint**: `/Manager/AssignMultipleLearnings` (POST)

**Flow Steps**:
1. Manager views team member detail page
2. Selects multiple learnings from checklist
3. Optionally sets custom due date
4. Clicks "Assign Selected Learnings"
5. System validates:
   - At least one learning selected
   - Manager identity
   - Team member belongs to manager
6. For each selected learning:
   - Check if already assigned
   - If not assigned:
     - Create new assignment
     - Set due date (custom or default 30 days)
     - Set status = "NotStarted"
     - Set progress = 0%
   - If already assigned:
     - Increment skipped counter
7. Save all new assignments
8. Display summary message:
   - "Successfully assigned X learning(s)"
   - "Y learning(s) were already assigned" (if any)
9. Log bulk assignment action
10. Redirect to team member detail page

**Business Rules**:
- If no learnings selected → Error: "Please select at least one learning"
- Due date is optional (default: 30 days)
- Duplicate assignments are skipped, not errored
- All assignments use same due date
- Assignments are created in single database transaction

**Success Messages**:
- Only new assignments: "Successfully assigned X learning(s) to [username]"
- Mixed new/existing: "Successfully assigned X learning(s). Y learning(s) were already assigned"
- All duplicates: "All selected learnings were already assigned to this user"

---

### 3.4 Team Progress Monitoring

#### 3.4.1 View Team Progress Overview
**Purpose**: Monitor learning progress across all team members

**Flow Steps**:
1. Manager accesses team overview
2. System aggregates data:
   - Total assignments per team member
   - Completion rates per team member
   - Overdue assignments
   - Recently completed assignments
3. Display metrics:
   - Team completion rate
   - Individual completion rates
   - Status distribution (NotStarted/InProgress/Completed)
   - Upcoming due dates
   - Overdue items (highlighted)

**Calculated Metrics**:
- **Team Completion Rate** = (Total Completed / Total Assigned) × 100
- **Individual Completion Rate** = (User Completed / User Total) × 100
- **Overdue Count** = Assignments where DueDate < Today AND Status != "Completed"

#### 3.4.2 Track Assignment Status
**Status Definitions**:
- **NotStarted**: Employee has not begun the learning
  - Progress = 0%
  - No completed date
- **InProgress**: Employee is actively working on learning
  - Progress = 1-99%
  - No completed date
- **Completed**: Employee has finished the learning
  - Progress = 100%
  - Completed date is set

---

### 3.5 Reporting Features

#### 3.5.1 Team Performance Reports
**Available Data**:
- Completion rates by team member
- Completion rates by learning category
- Average completion time
- Overdue assignments
- Recent completions

#### 3.5.2 Learning Category Analysis
**Metrics**:
- Most assigned categories
- Highest completion rate categories
- Average duration by category
- Team preferences analysis

---

## 4. Data Access Scope

### 4.1 What Manager CAN Access:
- Own profile information
- Direct team members (Employees with matching ManagerId)
- Team members' assignments
- Team members' learning progress
- All available learning content (for assignment purposes)

### 4.2 What Manager CANNOT Access:
- Other managers' teams
- Other managers' employees
- HR functionality
- Users outside their team
- System-wide reports
- Other managers' assignments

---

## 5. Security & Authorization

### 5.1 Authentication
- Claims-based authentication using `ClaimTypes.Name`
- Session-based login required
- Token validation on each request

### 5.2 Authorization
**Policy**: ManagerOnly
```csharp
[Authorize(Policy = "ManagerOnly")]
```

### 5.3 Data Filtering
All queries include:
```csharp
.Where(u => u.ManagerId == manager.Id)
```

### 5.4 Validation Checks
1. Manager identity verification
2. Team member ownership verification
3. Learning existence verification
4. Duplicate assignment checks

### 5.5 Security Responses
- Unauthorized user → 401 Unauthorized
- Non-team member access → 403 Forbidden
- Invalid ID → 404 Not Found

---

## 6. Key Features Summary

| Feature | Access | Description |
|---------|--------|-------------|
| View Dashboard | Own Team Only | Overview of team members |
| View Team Member Details | Own Team Only | Individual employee progress |
| Assign Single Learning | Own Team Only | Assign one learning at a time |
| Assign Multiple Learnings | Own Team Only | Bulk assignment with custom due date |
| Track Progress | Own Team Only | Monitor completion status |
| View Learning Catalog | All | Browse available learnings |
| Team Reports | Own Team Only | Completion and progress reports |

---

## 7. Business Rules Summary

1. Manager can only manage direct reports (ManagerId match)
2. Default due date for assignments: 30 days
3. Initial assignment status: "NotStarted"
4. Initial progress percentage: 0%
5. Duplicate assignments are prevented
6. Bulk assignments use single due date for all
7. All actions are logged for audit
8. Team members must have Role = "Employee"
9. Manager cannot assign learnings to themselves
10. Assignment dates use system timestamp (DateTime.Now)

---

## 8. Integration Points

- **Authentication**: Claims-based with Identity framework
- **Database**: AppDbContext with Entity Framework Core
- **Logging**: ILogger for audit trail
- **Authorization**: Policy-based (ManagerOnly)
- **Navigation**: Team member relationships via ManagerId foreign key

---

## 9. Error Handling

### 9.1 Validation Errors
- No learnings selected → Display error message
- Invalid team member ID → 404 Not Found
- Unauthorized team access → 403 Forbidden

### 9.2 Database Errors
- Assignment creation failure → Log and display error
- Concurrent update conflicts → Retry or notify user
- Connection failures → Generic error message

### 9.3 Business Logic Errors
- Duplicate assignment → Skip silently
- Missing due date → Use default (30 days)
- Invalid learning ID → Skip and continue with others

---

## 10. User Experience Flow

### 10.1 Typical Manager Workflow
1. **Login** → Manager Dashboard
2. **View Team** → List of team members
3. **Select Employee** → View assignments and progress
4. **Identify Gaps** → See which learnings are not assigned
5. **Assign Learning** → Single or bulk assignment
6. **Set Due Date** → Custom or default
7. **Monitor Progress** → Check status updates
8. **Follow Up** → Review overdue assignments

### 10.2 Assignment Workflow
```
Select Team Member 
    → Choose Learning(s) 
    → Set Due Date (optional) 
    → Confirm Assignment 
    → View Updated Status
```

---

## 11. Notifications (Future Enhancement)
- Email notification when learning assigned
- Reminder for approaching due dates
- Alert for overdue assignments
- Completion acknowledgment

---

## 12. Performance Considerations

- Team member queries include `.Include()` for assignments
- Dashboard loads only direct reports (filtered by ManagerId)
- Bulk operations use single transaction
- Indexes on ManagerId and UserId for query performance

---

## 13. Audit & Compliance

All manager actions are logged:
- Login/logout events
- Team member view actions
- Learning assignment actions (single and bulk)
- Search and filter operations

Log format:
```
Timestamp | Manager Username | Action | Target | Details
```

---

## 14. Future Enhancements

1. **Real-time Progress Updates**: WebSocket notifications
2. **Mobile App**: Manager mobile access
3. **Analytics Dashboard**: Advanced charts and graphs
4. **Export Reports**: PDF/Excel export
5. **Custom Templates**: Learning paths and templates
6. **Scheduling**: Schedule assignments for future dates
7. **Reminders**: Automated email reminders
8. **Feedback System**: Manager feedback on completion
9. **Team Comparison**: Compare team performance
10. **Learning Recommendations**: AI-suggested learnings

---

**Document Version**: 1.0  
**Last Updated**: 2025  
**System**: Learning Management System - .NET 8 Razor Pages  
**Role**: Manager
