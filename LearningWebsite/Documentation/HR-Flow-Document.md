# HR Flow - Functional Documentation
## Learning Management System

---

## 1. Overview
The HR role has the highest level of access in the system, responsible for managing all users, monitoring system-wide learning activities, and generating insights across the organization.

---

## 2. Access Control
- **Policy**: HROnly
- **Authorization**: Required for all HR controller actions
- **Role**: HR

---

## 3. Functional Flows

### 3.1 HR Dashboard (Index)
**Purpose**: Provides comprehensive overview of system-wide learning activities and user management

#### Flow Steps:
1. HR logs into the system
2. Navigates to HR Dashboard (`/HR/Index`)
3. System displays:
   - **User Metrics**:
     - Total Users
     - Total Employees
     - Total Managers
     - Total HR users
   - **Learning Assignment Metrics**:
     - Total Assignments
     - Completed Assignments
     - In-Progress Assignments
     - Not Started Assignments
     - Overall Completion Rate (%)
   - **Category Analysis**:
     - Assignments grouped by learning category
     - Completion rates per category
   - **Recent Assignments Table**:
     - Last 50 assignments with user, learning, status, and dates
   - **All Learnings**: Available for new assignments

#### Business Rules:
- Completion Rate = (Completed Assignments / Total Assignments) × 100
- Category stats calculate completion rate per category
- Dashboard logs HR access for audit purposes

---

### 3.2 User Management Flow

#### 3.2.1 View All Users
**Endpoint**: `/HR/ManageUsers`

**Flow Steps**:
1. HR clicks "Manage Users"
2. System retrieves all users with their manager relationships
3. Display sorted by:
   - Primary: Role (HR, Manager, Employee)
   - Secondary: Username (alphabetical)
4. Shows:
   - Username
   - Full Name
   - Email
   - Role
   - Manager (if applicable)
   - Actions (Edit, Delete)

---

#### 3.2.2 Create New User
**Endpoint**: `/HR/CreateUser` (GET & POST)

**Flow Steps**:
1. HR clicks "Create User" button
2. System displays form with fields:
   - Username (required)
   - Full Name
   - Email
   - Password (required)
   - Role (HR/Manager/Employee)
   - Manager (dropdown, if role is Employee)
3. HR fills in user details
4. System validates:
   - Username is unique
   - Password is provided
   - Required fields are filled
5. System hashes password using Identity Password Hasher
6. Creates new user in database
7. Logs creation action with username and role
8. Displays success message
9. Redirects to ManageUsers

**Validation Rules**:
- Username must be unique across all users
- Username and password are mandatory
- Email validation (format)
- If role is Employee, manager should be assigned

**Error Handling**:
- Duplicate username → Display error, keep form data
- Missing required fields → Display validation errors
- Database errors → Log and display generic error

---

#### 3.2.3 Edit Existing User
**Endpoint**: `/HR/EditUser/{id}` (GET & POST)

**Flow Steps**:
1. HR clicks "Edit" on a user
2. System loads user details into form
3. System populates managers dropdown (excluding current user)
4. HR modifies:
   - Username
   - Full Name
   - Email
   - Role
   - Manager assignment
   - New Password (optional)
5. System validates username uniqueness (excluding current user)
6. If new password provided, hash and update
7. Update user record
8. Log modification action
9. Display success message
10. Redirect to ManageUsers

**Business Rules**:
- Password update is optional
- If password changed, must hash before storing
- Username uniqueness check excludes current user
- Manager dropdown excludes current user to prevent circular assignment

---

#### 3.2.4 Delete User
**Endpoint**: `/HR/DeleteUser/{id}` (POST)

**Flow Steps**:
1. HR clicks "Delete" on a user
2. System confirms deletion
3. Deletes user record
4. Cascading deletions:
   - Remove all learning assignments for this user
   - Update employees who report to this manager (if applicable)
5. Log deletion action
6. Display success message
7. Redirect to ManageUsers

**Business Rules**:
- Cannot delete users with active assignments (depending on business requirements)
- Cascading deletes must maintain referential integrity
- Audit log must record deletion

---

### 3.3 Learning Management Flow

#### 3.3.1 Manage Learning Content
**Purpose**: HR can view and manage all learning materials

**Flow Steps**:
1. Access learning management section
2. View all available learnings:
   - Title
   - Description
   - Category
   - Duration (hours)
3. Create new learning content
4. Edit existing learning content
5. Delete unused learning content

#### 3.3.2 Assign Learning to Users
**Purpose**: HR can assign any learning to any user

**Flow Steps**:
1. From dashboard, select learning and user
2. Set due date (default: 30 days)
3. Create assignment with:
   - Status: "NotStarted"
   - Progress: 0%
   - Assigned Date: Current timestamp
4. Check for duplicate assignments
5. Save and notify user

---

### 3.4 Reporting & Analytics Flow

#### 3.4.1 Completion Rate Analysis
**Metrics Calculated**:
- Overall completion rate
- Per-category completion rates
- Per-user completion rates
- Per-manager team completion rates

#### 3.4.2 Assignment Tracking
**Data Displayed**:
- All assignments (last 50 on dashboard)
- Filterable by:
  - Status (NotStarted, InProgress, Completed)
  - User
  - Learning
  - Date range
  - Category

---

### 3.5 Audit & Logging
**All HR actions are logged with**:
- Timestamp
- HR username
- Action performed (Create, Update, Delete, View)
- Target entity (User, Learning, Assignment)
- Changes made (for updates)

---

## 4. Data Access
HR has full access to:
- All users (HR, Managers, Employees)
- All learning content
- All assignments across organization
- All historical data
- All metrics and reports

---

## 5. Key Features Summary
| Feature | Access Level | Description |
|---------|-------------|-------------|
| View Dashboard | Full | System-wide metrics and insights |
| Manage Users | Full | Create, Read, Update, Delete all users |
| Manage Learnings | Full | Create, Read, Update, Delete learning content |
| Assign Learnings | Full | Assign to any user |
| View Assignments | Full | All assignments across organization |
| Generate Reports | Full | All analytics and completion rates |
| Audit Logs | Full | View all system activities |

---

## 6. Integration Points
- **Authentication System**: Identity framework for user management
- **Database**: AppDbContext for data operations
- **Password Hashing**: IPasswordHasher for secure password storage
- **Logging**: ILogger for audit trail
- **Authorization**: Policy-based (HROnly)

---

## 7. Business Rules Summary
1. HR must be authenticated and authorized
2. All user operations are logged for audit
3. Passwords must be hashed before storage
4. Username must be unique
5. Completion rate calculations are real-time
6. Dashboard shows last 50 assignments for performance
7. Manager dropdown excludes current user being edited
8. Delete operations must maintain referential integrity
9. Category stats group by learning category
10. All timestamps use system time (DateTime.Now)

---

## 8. Error Handling
- Invalid user ID → 404 Not Found
- Duplicate username → Validation error with message
- Missing required fields → ModelState errors
- Database failures → Logged and generic error shown
- Unauthorized access → 403 Forbidden

---

## 9. Future Enhancements
- Export reports to Excel/PDF
- Bulk user import from CSV
- Email notifications for assignments
- Advanced filtering on dashboard
- Custom date range for metrics
- User activity timeline
- Scheduled reports

---

**Document Version**: 1.0  
**Last Updated**: 2025  
**System**: Learning Management System - .NET 8 Razor Pages
