# Learning Website - ASP.NET Core MVC

A comprehensive learning management system built with ASP.NET Core 8.0 MVC, Entity Framework Core, and SQL Server.

## 🎯 Features

### User Management
- **Role-Based Access Control**: Employee, Manager, and HR roles
- **Authentication**: Cookie-based authentication with secure password hashing
- **Authorization Policies**: Fine-grained access control per role

### Assessment System
- **Interactive Assessments**: Multiple-choice questions for learning validation
- **Real-time Scoring**: Instant feedback on assessment completion
- **Result Tracking**: Detailed answer tracking and review functionality
- **Passing Criteria**: Configurable passing score (60% default)

### Certificate Generation
- **Automated Certificates**: Generated upon successful assessment completion (≥60%)
- **Professional Design**: Landscape A4 format with corporate branding
- **Print & Share**: One-click printing and social media sharing
- **Certificate Number**: Unique certificate tracking number
- **Score Display**: Achievement score prominently displayed

### Learning Resources
- **Course Catalog**: Categorized learning materials
- **Assignment Management**: Managers/HR can assign courses to employees
- **Progress Tracking**: Monitor learning progress and completion
- **Filtering**: Category-based course filtering

### Dashboard & Reporting
- **Role-Specific Dashboards**:
  - **Employee**: My assignments, certificates, progress
  - **Manager**: Team assignments, completion rates
  - **HR**: Organization-wide learning analytics
- **API Endpoints**: RESTful APIs for dashboard data

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity (Cookie-based)
- **Frontend**: Razor Pages, Bootstrap 5, Font Awesome
- **Testing**: xUnit, In-Memory Database

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (or SQL Server)
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/rameshdsofteng/LearningWebsite.git
cd LearningWebsite
```

### 2. Update Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LearningWebsiteMVCDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Run Migrations
```bash
dotnet ef database update
```

### 4. Build and Run
```bash
dotnet build
dotnet run
```

The application will be available at `https://localhost:7114`

## 👤 Default Users

The system seeds three default users for testing:

| Email | Password | Role |
|-------|----------|------|
| employee@example.com | Employee123! | Employee |
| manager@example.com | Manager123! | Manager |
| hr@example.com | HR123! | HR |

## 📁 Project Structure

```
LearningWebsite/
├── Controllers/           # MVC Controllers
│   ├── AccountController.cs
│   ├── AssessmentController.cs
│   ├── CertificatesController.cs
│   ├── EmployeeController.cs
│   ├── HRController.cs
│   ├── ManagerController.cs
│   └── Api/              # API Controllers
│       ├── AssignmentsController.cs
│       ├── DashboardController.cs
│       └── LearningsController.cs
├── Models/               # Domain Models & ViewModels
│   ├── ApplicationUser.cs
│   ├── Assessment*.cs
│   ├── Certificate.cs
│   ├── Learning*.cs
│   └── Question.cs
├── Data/                 # Database Context & Initialization
│   ├── AppDbContext.cs
│   ├── DbInitializer.cs
│   ├── QuestionDataInitializer.cs
│   └── DatabaseCleaner.cs
├── Views/                # Razor Views
│   ├── Account/
│   ├── Assessment/
│   ├── Certificates/
│   ├── Employee/
│   ├── HR/
│   ├── Manager/
│   └── Shared/
├── wwwroot/              # Static Files
│   ├── css/
│   ├── js/
│   └── lib/
├── Migrations/           # EF Core Migrations
└── Documentation/        # Project Documentation
    ├── Employee-Flow-Document.md
    ├── Manager-Flow-Document.md
    └── HR-Flow-Document.md
```

## 📚 Documentation

- **[API Testing Guide](API_TESTING_GUIDE.md)** - REST API endpoints and testing
- **[Testing Guide](TESTING_GUIDE.md)** - Unit and integration testing
- **[Code Cleanup Report](CODE_CLEANUP_REPORT.md)** - Project cleanup analysis
- **[Employee Flow](Documentation/Employee-Flow-Document.md)** - Employee user journey
- **[Manager Flow](Documentation/Manager-Flow-Document.md)** - Manager user journey
- **[HR Flow](Documentation/HR-Flow-Document.md)** - HR admin user journey

## 🔒 Security Features

- **Password Hashing**: ASP.NET Core Identity password hasher
- **Role-Based Authorization**: `[Authorize]` attributes with role policies
- **HTTPS**: Enforced in production
- **CSRF Protection**: Built-in anti-forgery tokens
- **SQL Injection Prevention**: Parameterized queries via EF Core

## 🧪 Testing

Run unit tests:
```bash
cd LearningWebsite.Tests
dotnet test
```

## 🗄️ Database Management

### Reset Database (Delete all data and reseed)
Set in `appsettings.json`:
```json
{
  "ResetDatabase": true
}
```
Then restart the application. **Remember to set back to `false`!**

### Manual Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 📈 Key Features Implementation

### Assessment Flow
1. Employee views assigned learnings
2. Takes assessment (MCQ)
3. Submits answers
4. System calculates score
5. If ≥60%, generates certificate
6. Employee can view/print certificate

### Assignment Flow
1. HR/Manager selects learning
2. Assigns to employee(s)
3. Sets due date
4. Employee receives assignment
5. Tracks completion status

### Certificate Generation
- Automatically triggered on passing assessment
- Contains: Employee name, course, score, date, certificate number
- Printable in landscape A4 format
- Shareable on LinkedIn/Twitter

## 🛣️ Roadmap

- [ ] Email notifications
- [ ] Certificate expiry dates
- [ ] Learning paths (course sequences)
- [ ] Bulk user upload
- [ ] Advanced reporting
- [ ] Mobile app

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- **Ramesh D** - *Initial work* - [rameshdsofteng](https://github.com/rameshdsofteng)

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Bootstrap for responsive UI components
- Font Awesome for icons

## 📞 Support

For issues and questions:
- **GitHub Issues**: [Create an issue](https://github.com/rameshdsofteng/LearningWebsite/issues)
- **Email**: rameshdsofteng@example.com

---

**Built with ❤️ using ASP.NET Core**