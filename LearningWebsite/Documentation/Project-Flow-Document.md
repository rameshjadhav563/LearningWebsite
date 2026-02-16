# Project Flow Document - Learning Management System
## ASP.NET Core 8.0 MVC Application

---

## Table of Contents
1. [System Overview](#1-system-overview)
2. [Architecture](#2-architecture)
3. [Technology Stack](#3-technology-stack)
4. [Application Flow](#4-application-flow)
5. [Database Schema](#5-database-schema)
6. [Authentication & Authorization](#6-authentication--authorization)
7. [User Journey Flows](#7-user-journey-flows)
8. [API Endpoints](#8-api-endpoints)
9. [Key Features](#9-key-features)
10. [Project Structure](#10-project-structure)
11. [Setup & Configuration](#11-setup--configuration)
12. [Testing Strategy](#12-testing-strategy)

---

## 1. System Overview

### 1.1 Purpose
The Learning Management System (LMS) is a comprehensive web application designed to facilitate organizational learning and training. It enables HR and Managers to assign courses to employees, track progress, conduct assessments, and issue certificates upon successful completion.

### 1.2 Key Stakeholders
- **Employees**: End-users who complete assigned learning modules and assessments
- **Managers**: Team leads who assign courses to their direct reports and monitor progress
- **HR**: Human Resources personnel with organization-wide access and administrative capabilities

### 1.3 Business Goals
- Streamline employee learning and development
- Track training completion and progress
- Automate certificate generation for completed courses
- Provide role-based dashboards for different user types
- Enable data-driven decisions on learning effectiveness

---

## 2. Architecture

### 2.1 Application Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │  Views   │  │ Layouts  │  │ Partials │  │   wwwroot │   │
│  │ (Razor)  │  │          │  │          │  │  (Static) │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                     Controller Layer                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │ Account  │  │ Employee │  │ Manager  │  │    HR     │   │
│  │Controller│  │Controller│  │Controller│  │Controller │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐                  │
│  │Assessment│  │Certificate│ │API/       │                  │
│  │Controller│  │Controller│  │Controllers│                  │
│  └──────────┘  └──────────┘  └──────────┘                  │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                      Business Logic                          │
│  ┌────────────────────────────────────────────────────┐     │
│  │  Models (Entities, ViewModels, DTOs)              │     │
│  │  - ApplicationUser, Learning, Assessment, etc.     │     │
│  └────────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                      Data Access Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ AppDbContext │  │ DbInitializer│  │ Migrations   │      │
│  │  (EF Core)   │  │              │  │              │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                          │
│             SQL Server (LocalDB/SQL Server)                  │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Design Patterns
- **MVC Pattern**: Clear separation of concerns between Model, View, and Controller
- **Repository Pattern**: Abstracted through EF Core DbContext
- **Dependency Injection**: Built-in ASP.NET Core DI container
- **Authorization Policies**: Role-based access control using ASP.NET Core policies

---

## 3. Technology Stack

### 3.1 Backend
- **Framework**: ASP.NET Core 8.0 (MVC)
- **Language**: C# 12
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server / LocalDB
- **Authentication**: ASP.NET Core Cookie Authentication
- **Password Hashing**: ASP.NET Core Identity Password Hasher

### 3.2 Frontend
- **View Engine**: Razor Views
- **CSS Framework**: Bootstrap 5
- **Icons**: Font Awesome
- **JavaScript**: Vanilla JS (for client-side interactions)

### 3.3 Testing
- **Framework**: xUnit
- **Database**: In-Memory Database (EF Core InMemory provider)
- **Coverage**: Unit Tests, Integration Tests

### 3.4 Tools & Infrastructure
- **Version Control**: Git (GitHub)
- **IDE**: Visual Studio 2026
- **Package Manager**: NuGet
- **Build System**: MSBuild / dotnet CLI

---

## 4. Application Flow

### 4.1 Request Pipeline
```
HTTP Request
    ↓
HTTPS Redirection Middleware
    ↓
Static Files Middleware (wwwroot)
    ↓
Routing Middleware
    ↓
Authentication Middleware (Cookie)
    ↓
Authorization Middleware (Role-based policies)
    ↓
MVC Middleware (Controller → Action)
    ↓
Action Execution
    ↓
View Rendering / JSON Response
    ↓
HTTP Response
```

### 4.2 Startup Configuration Flow
1. **Program.cs** configures services and middleware
2. **Database Initialization**: On application start, seed initial data
3. **Service Registration**: 
   - DbContext with SQL Server
   - Cookie Authentication
   - Authorization Policies (EmployeeOnly, ManagerOnly, HROnly)
   - Password Hasher
4. **Middleware Pipeline**: Configured in correct order for security and functionality

---

## 5. Database Schema

### 5.1 Entity Relationship Diagram
```
┌───────────────────┐
│ ApplicationUser   │
│───────────────────│
│ Id (PK)          │◄────┐
│ UserName         │     │
│ PasswordHash     │     │
│ Role             │     │
│ FullName         │     │
│ Email            │     │
│ ManagerId (FK)   │─────┘ (Self-referencing)
└───────────────────┘
        │ 1
        │
        │ *
┌───────────────────┐
│LearningAssignment│
│───────────────────│        ┌───────────────────┐
│ Id (PK)          │    ┌───►│    Learning       │
│ UserId (FK)      │────┘    │───────────────────│
│ LearningId (FK)  │─────────►Id (PK)           │
│ AssignedDate     │         │Title             │
│ DueDate          │         │Description       │
│ IsCompleted      │         │Category          │
│ CompletedDate    │         │DurationHours     │
└───────────────────┘         │PassingScore      │
                              └───────────────────┘
                                      │ 1
                                      │
                                      │ *
┌───────────────────┐                 │
│    Question       │                 │
│───────────────────│                 │
│ Id (PK)          │◄────────────────┘
│ LearningId (FK)  │
│ QuestionText     │
│ OptionA          │
│ OptionB          │
│ OptionC          │
│ OptionD          │
│ CorrectOption    │
└───────────────────┘

┌───────────────────┐         ┌───────────────────────┐
│ AssessmentResult  │    ┌───►│AssessmentAnswerDetail │
│───────────────────│    │    │───────────────────────│
│ Id (PK)          │────┘    │ Id (PK)              │
│ UserId (FK)      │         │ AssessmentResultId   │
│ LearningId (FK)  │         │ QuestionId (FK)      │
│ Score            │         │ SelectedOption       │
│ IsPassed         │         │ IsCorrect            │
│ CompletedAt      │         └───────────────────────┘
└───────────────────┘

┌───────────────────┐
│   Certificate     │
│───────────────────│
│ Id (PK)          │
│ UserId (FK)      │
│ LearningId (FK)  │
│ CertificateNumber│
│ IssuedDate       │
│ Score            │
└───────────────────┘
```

### 5.2 Key Entities

#### ApplicationUser
- **Purpose**: Stores user account information
- **Key Fields**: Id, UserName, PasswordHash, Role, ManagerId
- **Relationships**: Self-referencing (Manager-Employee), One-to-Many with Assignments

#### Learning
- **Purpose**: Represents a course/learning module
- **Key Fields**: Id, Title, Description, Category, DurationHours, PassingScore
- **Relationships**: One-to-Many with Questions, Assignments, Certificates

#### LearningAssignment
- **Purpose**: Links users to assigned courses
- **Key Fields**: Id, UserId, LearningId, AssignedDate, DueDate, IsCompleted
- **Relationships**: Many-to-One with User and Learning

#### Question
- **Purpose**: Stores assessment questions for each learning module
- **Key Fields**: Id, LearningId, QuestionText, Options (A-D), CorrectOption
- **Relationships**: Many-to-One with Learning

#### AssessmentResult
- **Purpose**: Records assessment completion and scoring
- **Key Fields**: Id, UserId, LearningId, Score, IsPassed, CompletedAt
- **Relationships**: One-to-Many with AssessmentAnswerDetails

#### Certificate
- **Purpose**: Stores issued certificates
- **Key Fields**: Id, UserId, LearningId, CertificateNumber, IssuedDate, Score
- **Relationships**: Many-to-One with User and Learning

---

## 6. Authentication & Authorization

### 6.1 Authentication Flow
```
1. User navigates to Login page (/Account/Login)
   ↓
2. Submits credentials (Username, Password)
   ↓
3. AccountController validates:
   - User exists in database
   - Password hash matches (using PasswordHasher)
   ↓
4. On success:
   - Create Claims (UserId, UserName, Role)
   - Create ClaimsIdentity with CookieAuthentication
   - Sign in user (sets authentication cookie)
   ↓
5. Redirect to role-specific dashboard
   - Employee → /Employee/Index
   - Manager → /Manager/Index
   - HR → /HR/Index
```

### 6.2 Authorization Policies
```csharp
EmployeeOnly: RequireRole("Employee")
ManagerOnly:  RequireRole("Manager")
HROnly:       RequireRole("HR")
```

### 6.3 Controller Protection
- All controllers use `[Authorize(Policy = "RolePolicy")]` attribute
- Unauthorized access redirects to `/Account/AccessDenied`
- Non-authenticated access redirects to `/Account/Login`

---

## 7. User Journey Flows

### 7.1 Employee Journey
```
Login → Employee Dashboard
  ↓
View Assigned Learnings
  ↓
Click "Start Learning" → Learning Details
  ↓
Click "Take Assessment"
  ↓
Answer Questions (Multiple Choice)
  ↓
Submit Assessment
  ↓
View Results:
  - If Score ≥ 60%: Certificate Generated (View/Print/Share)
  - If Score < 60%: Retake Option Available
  ↓
Mark Assignment as Completed
  ↓
View Certificate (if earned)
```

### 7.2 Manager Journey
```
Login → Manager Dashboard
  ↓
View Team Members
  ↓
Select Team Member → Assign Learning
  ↓
Choose Learning Module + Set Due Date
  ↓
Submit Assignment
  ↓
Monitor Team Progress:
  - View completion rates
  - Track individual progress
  - Review certificates earned
```

### 7.3 HR Journey
```
Login → HR Dashboard
  ↓
Organization-wide View:
  - Total employees
  - Total assignments
  - Completion rates
  - Department metrics
  ↓
Assign Learnings (any employee)
  ↓
View Analytics:
  - Top performers
  - Completion trends
  - Certificate statistics
  ↓
Manage Learning Catalog
```

---

## 8. API Endpoints

### 8.1 Dashboard APIs
```
GET /api/Dashboard/GetEmployeeDashboard?userId={id}
  - Returns: Assignments, completion stats, certificates

GET /api/Dashboard/GetManagerDashboard?managerId={id}
  - Returns: Team metrics, individual progress, completion rates

GET /api/Dashboard/GetHRDashboard
  - Returns: Organization-wide statistics
```

### 8.2 Learning APIs
```
GET /api/Learnings/GetByCategoryForUser?userId={id}&category={cat}
  - Returns: Available learnings filtered by category

GET /api/Learnings/Details/{id}
  - Returns: Learning details with assignments
```

### 8.3 Assignment APIs
```
GET /api/Assignments/GetAssignmentsForUser?userId={id}
  - Returns: User's assigned learnings

POST /api/Assignments/AssignLearning
  - Body: { userId, learningId, dueDate }
  - Returns: Created assignment

PUT /api/Assignments/MarkAsCompleted/{id}
  - Marks assignment as completed
```

---

## 9. Key Features

### 9.1 Assessment System
- **Question Bank**: Each learning module has 5-10 MCQ questions
- **Randomization**: Questions can be presented in random order
- **Instant Scoring**: Automatic calculation upon submission
- **Answer Tracking**: Detailed record of selected options and correctness
- **Pass/Fail Criteria**: 60% passing score (configurable per learning)
- **Retake Option**: Unlimited attempts for failed assessments

### 9.2 Certificate Generation
- **Automatic Generation**: Certificate created when Score ≥ PassingScore
- **Unique Identifier**: CertificateNumber format: `CERT-{LearningId}-{UserId}-{Timestamp}`
- **Professional Design**: 
  - A4 Landscape format
  - Corporate branding
  - Employee name, course name, score, date
  - Print-friendly CSS
- **Actions**: View, Print, Share on social media

### 9.3 Role-Based Dashboards
- **Employee Dashboard**:
  - Welcome message
  - My assignments (active, completed)
  - Progress tracking
  - Earned certificates
  
- **Manager Dashboard**:
  - Team overview
  - Individual team member progress
  - Assignment capabilities
  - Completion metrics
  
- **HR Dashboard**:
  - Organization-wide statistics
  - Total employees, assignments, completions
  - Department/team comparisons
  - Certificate issuance rates

### 9.4 Learning Management
- **Categorization**: Learnings grouped by category
- **Filtering**: Filter by category on learning list
- **Assignment Tracking**: Due dates, completion status
- **Progress Indicators**: Visual progress bars

---

## 10. Project Structure

```
LearningWebsite/
│
├── Controllers/
│   ├── AccountController.cs        (Login, Logout, Register)
│   ├── EmployeeController.cs       (Employee dashboard, actions)
│   ├── ManagerController.cs        (Manager dashboard, team management)
│   ├── HRController.cs             (HR dashboard, org-wide actions)
│   ├── AssessmentController.cs     (Take assessment, submit, results)
│   ├── CertificatesController.cs   (View, print certificates)
│   ├── HomeController.cs           (Public home, error pages)
│   └── Api/
│       ├── DashboardController.cs  (Dashboard APIs)
│       ├── LearningsController.cs  (Learning APIs)
│       └── AssignmentsController.cs(Assignment APIs)
│
├── Models/
│   ├── ApplicationUser.cs          (User entity)
│   ├── Learning.cs                 (Course entity)
│   ├── LearningAssignment.cs       (Assignment entity)
│   ├── Question.cs                 (Question entity)
│   ├── AssessmentResult.cs         (Assessment result entity)
│   ├── AssessmentAnswerDetail.cs   (Answer detail entity)
│   ├── Certificate.cs              (Certificate entity)
│   ├── AssessmentViewModel.cs      (Assessment UI model)
│   ├── LoginViewModel.cs           (Login form model)
│   ├── TeamMetricsViewModel.cs     (Manager metrics model)
│   └── PaginatedList.cs            (Pagination helper)
│
├── Views/
│   ├── Account/                    (Login, Register views)
│   ├── Employee/                   (Employee views)
│   ├── Manager/                    (Manager views)
│   ├── HR/                         (HR views)
│   ├── Assessment/                 (Assessment views)
│   ├── Certificates/               (Certificate views)
│   ├── Home/                       (Public views)
│   └── Shared/                     (Layout, partials)
│       ├── _Layout.cshtml          (Main layout)
│       └── _LoginPartial.cshtml    (Login/logout partial)
│
├── Data/
│   ├── AppDbContext.cs             (EF Core DbContext)
│   ├── DbInitializer.cs            (Seed users)
│   ├── LearningDataInitializer.cs  (Seed learnings)
│   ├── QuestionDataInitializer.cs  (Seed questions)
│   └── DatabaseCleaner.cs          (Reset database utility)
│
├── Migrations/                     (EF Core migrations)
│
├── wwwroot/                        (Static files: CSS, JS, images)
│
├── Documentation/
│   ├── Employee-Flow-Document.md   (Employee functional doc)
│   ├── Manager-Flow-Document.md    (Manager functional doc)
│   ├── HR-Flow-Document.md         (HR functional doc)
│   └── Project-Flow-Document.md    (This document)
│
├── Program.cs                      (Application entry point)
├── appsettings.json                (Configuration)
└── LearningWebsite.csproj          (Project file)
```

---

## 11. Setup & Configuration

### 11.1 Prerequisites
- .NET 8.0 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2022+ or VS Code

### 11.2 Installation Steps

1. **Clone Repository**
   ```bash
   git clone https://github.com/rameshjadhav563/LearningWebsite.git
   cd LearningWebsite
   ```

2. **Configure Database Connection**
   - Edit `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LearningWebsiteDb;Trusted_Connection=True;"
     }
     ```

3. **Restore Packages**
   ```bash
   dotnet restore
   ```

4. **Apply Migrations**
   ```bash
   dotnet ef database update
   ```
   
   *Database is automatically created and seeded on first run*

5. **Run Application**
   ```bash
   dotnet run
   ```
   
   Navigate to: `https://localhost:5001`

### 11.3 Default Users (Seeded)

| Username    | Password | Role     |
|-------------|----------|----------|
| emp1        | Pass@123 | Employee |
| emp2        | Pass@123 | Employee |
| mgr1        | Pass@123 | Manager  |
| hr1         | Pass@123 | HR       |

### 11.4 Configuration Options

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "ResetDatabase": false,  // Set to true to reset DB on startup
  "Logging": { ... }
}
```

---

## 12. Testing Strategy

### 12.1 Test Project Structure
```
LearningWebsite.Tests/
├── Unit/
│   ├── Controllers/
│   ├── Models/
│   └── Services/
└── Integration/
    └── CertificationFlowIntegrationTests.cs
```

### 12.2 Testing Approach
- **Unit Tests**: Test individual controller actions, model validations
- **Integration Tests**: Test complete user flows (e.g., certification flow)
- **Database**: In-Memory Database for isolated testing
- **Framework**: xUnit with Assert statements

### 12.3 Running Tests
```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter FullyQualifiedName~CertificationFlowIntegrationTests

# With coverage
dotnet-coverage collect -f cobertura -o coverage.xml dotnet test
```

---

## 13. Security Considerations

### 13.1 Authentication Security
- **Password Hashing**: Uses ASP.NET Core Identity PasswordHasher (PBKDF2)
- **Cookie Security**: HttpOnly, Secure flags enabled
- **Anti-CSRF**: Razor automatically includes anti-forgery tokens

### 13.2 Authorization
- **Role-Based Access Control**: Strict policy enforcement
- **Data Isolation**: Users can only access their own data
- **Manager Scope**: Managers can only manage direct reports

### 13.3 Input Validation
- **Model Validation**: DataAnnotations on all input models
- **SQL Injection**: Protected by EF Core parameterized queries
- **XSS Protection**: Razor engine automatically encodes output

---

## 14. Performance Optimization

### 14.1 Database Optimization
- **Eager Loading**: `.Include()` for related entities
- **Async Operations**: All database operations use async/await
- **Indexes**: Primary keys and foreign keys indexed by default

### 14.2 Caching Strategy
- **Static Files**: Browser caching enabled
- **In-Memory Caching**: Can be implemented for frequently accessed data

---

## 15. Future Enhancements

### 15.1 Planned Features
- **Content Management**: Upload learning materials (PDFs, videos)
- **Notifications**: Email/SMS notifications for assignments
- **Advanced Analytics**: Interactive charts and reports
- **Mobile App**: Native mobile applications
- **Gamification**: Badges, leaderboards, points system
- **Discussion Forums**: Collaborative learning spaces

### 15.2 Technical Improvements
- **API Versioning**: RESTful API with versioning
- **Real-time Updates**: SignalR for live notifications
- **Microservices**: Split into smaller services
- **Cloud Deployment**: Azure App Service deployment
- **CI/CD Pipeline**: Automated testing and deployment

---

## 16. Support & Maintenance

### 16.1 Logging
- **Console Logging**: Enabled in development
- **File Logging**: Can be configured for production
- **Error Pages**: Custom error pages in production

### 16.2 Monitoring
- **Health Checks**: Can be added for production monitoring
- **Application Insights**: Azure monitoring integration possible

---

## 17. License & Contact

### 17.1 Repository
**GitHub**: https://github.com/rameshjadhav563/LearningWebsite

### 17.2 Documentation
For role-specific functional documentation, refer to:
- [Employee Flow Document](Employee-Flow-Document.md)
- [Manager Flow Document](Manager-Flow-Document.md)
- [HR Flow Document](HR-Flow-Document.md)

---

## Document Version
- **Version**: 1.0
- **Last Updated**: January 2026
- **Author**: Development Team
- **Status**: Active
