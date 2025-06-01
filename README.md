# 📚 API for Platform Education

This project is a RESTful API designed to support an educational platform, providing robust features for managing courses, sections, videos, and exams. The API is built with scalability, maintainability, and security in mind.

---

## 🚀 Key Features

- **CRUD Operations**: Create, Read, Update, and Delete operations for managing educational resources like courses, students, and sections.
- **Option Pattern**: Simplifies configuration management and keeps the app settings organized.
- **Refresh Tokens**: Secure token-based authentication with refresh tokens to extend user sessions.
- **CORS**: Cross-Origin Resource Sharing configured to control API access from different origins.
- **Error Handling with Result Pattern**: Consistent and structured error handling across the API.
- **Distributed Caching** (Questions Service): Enhances performance and scalability for frequently accessed questions.
- **Hangfire & Recurring Jobs**: Background job processing for tasks like email notifications or cleanup tasks.
- **Permission-Based Authentication**: Secure access control using roles and permissions..
- **Pagination, Health Checks, and Rate Limiting**: Efficient data loading, API health monitoring, and request throttling to ensure stability and performance.

---

## 🛠️ Technologies Used

- **.NET Core / ASP.NET Core**
- **Entity Framework Core**
- **SQL Server**
- **Dependency Injection**
- **Option Pattern**
- **Swagger** (API documentation)
- **Hangfire**
- **Distributed Caching (e.g., Redis)**
- **Result Pattern for Error Handling**
- **Authentication & Authorization (JWT + Roles)**
- **Rate Limiting Middleware**
- **Health Checks**

---

## 📂 Project Structure

API for Platform Education
│
├── Contracts/ ← API contracts and interfaces
├── Controllers/ ← API endpoints
├── Dto/ ← Data Transfer Objects (DTOs)
├── Models/ ← Domain models
├── Services/ ← Business logic and services
├── Configurations/ ← App settings using Option Pattern
├── BackgroundJobs/ ← Hangfire recurring jobs
├── Templets/ ← API templates (email, notifications, etc.)
├── appsettings.json ← Configuration file
└── Program.cs ← Application entry point
