# 🚀 TrustPath – Lead Management Web Service API

**TrustPath Web Service** is a RESTful ASP.NET Core Web API designed to power the TrustPath Lead Management System.

It provides secure and scalable API services for managing business leads, lead sources, agents, follow-ups, pipeline stages, activities, and reports.

The API follows a clean layered architecture using Entity Framework Core, JWT authentication, role-based authorization, and centralized logging.

## ✨ Key Features

- **Authentication & Authorization:** Secure JWT-based authentication and role-based access control.
- **Lead Management:** Create, update, retrieve, and manage business leads.
- **Lead Source Management:** Manage different sources from which leads are generated.
- **Agent Management:** Manage sales agents and their assigned responsibilities.
- **Follow-Up Management:** Schedule, track, and manage lead follow-up activities.
- **Pipeline Stage Management:** Organize leads into different sales pipeline stages.
- **Activity Management:** Track activities and interactions related to leads.
- **Reports & Analytics:** Generate reports to monitor lead performance and business activities.
- **Role-Based Access:** Restrict API functionality based on user roles.
- **Centralized Logging:** Maintain application logs for monitoring and troubleshooting.
- **Layered Architecture:** Maintain clean separation between business logic, data access, and API endpoints.

## 🛠️ Technology Stack

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server (database provider, if configured)
- JWT Authentication
- RESTful API Architecture
- LINQ
- Dependency Injection
- Repository Pattern
- Layered Architecture

## 🏗️ Project Architecture

The solution follows a layered architecture to improve maintainability, scalability, and separation of concerns.

```text
TrustPath-Web-Service/
│
├── TrustPathWebService.Domain/
│
├── TrustPathWebService.Infrastructure/
│
├── TrustPathWebService.Persistance/
│
├── TrustPathWebService.Repository/
│
├── TrustPathWebService.Service/
│
├── TrustPathWebService/
│
└── TrustPathWebService.sln
```

### Layer Responsibilities

| Project        | Responsibility                                           |
| -------------- | -------------------------------------------------------- |
| Domain         | Contains domain entities and application models.         |
| Infrastructure | Handles infrastructure-related functionality.            |
| Persistance    | Manages database context and persistence operations.     |
| Repository     | Handles data access and database operations.             |
| Service        | Contains business logic and application services.        |
| WebService     | Exposes RESTful API endpoints and handles HTTP requests. |

## 🔐 Security

- JWT-based authentication.
- Role-based authorization.
- Protected API endpoints.
- Secure API access.
- Centralized authentication and authorization.
- Secure application configuration.

## ⚙️ Getting Started

### Prerequisites

- Visual Studio
- Compatible .NET SDK
- SQL Server (if configured)
- Entity Framework Core tools
- NuGet Package Manager

### Installation

**1. Clone the repository**

```bash
git clone https://github.com/drashti-shastra-soft/TrustPath-Web-Service.git
```

**2. Navigate to the project directory**

```bash
cd TrustPath-Web-Service
```

**3. Open the solution**

Open the solution file in Visual Studio:

```text
TrustPathWebService.sln
```

**4. Restore NuGet Packages**

```bash
dotnet restore
```

**5. Configure Application Settings**

Configure the required database connection string, JWT settings, and other application configurations.

Never commit production credentials, passwords, or JWT secrets to GitHub.

**6. Build the Project**

```bash
dotnet build
```

**7. Run the API**

Select `TrustPathWebService` as the startup project in Visual Studio and run the application.

## 🔗 Related Project

### TrustPath – Lead Management Web Console

The TrustPath Web Console communicates with this Web Service to provide the user interface for managing leads, agents, follow-ups, activities, and reports.

**GitHub Repository:**

[https://github.com/drashti-shastra-soft/TrustPath-Web-Console](https://github.com/drashti-shastra-soft/TrustPath-Web-Console)

## 🎯 Project Objective

The primary objective of TrustPath Web Service is to provide a centralized, secure, and scalable backend solution for lead management.

It simplifies lead tracking, sales pipeline management, agent coordination, follow-up scheduling, and business reporting through structured API services.
