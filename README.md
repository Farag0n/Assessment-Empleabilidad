# Assessment_M6 - Courses & Lessons Management API

## 📖 Description

A robust RESTful API built with ASP.NET Core 9.0 for managing lessons, courses, and users with JWT-based authentication and role-based authorization. This application follows Clean Architecture principles and provides complete CRUD operations with secure access control.

---

## 🏗️ Architecture

### Clean Architecture Layers

```
Assessment-Empleabilidad/
├── Assessment-Empleabilidad.Api/              
│   ├── Controllers/               
│   ├── Program.cs                 
│   └── appsettings.json           
├── Assessment_M6.Application/     
│   ├── DTOs/                     
│   ├── Interfaces/               
│   └── Services/                 
├── Assessment-Empleabilidad.Domain/         
│   ├── Entities/                 
│   ├── Enums/                    
│   └── Interfaces/               
├── Assessment-Empleabilidad.Infrastructure/  
│   ├── Data/                     
│   ├── Repositories/             
│   └── Extensions/
└── Assessment-Empleabilidad.Test/            
```

---

## 🚀 Getting Started

### Prerequisites

* .NET 9.0 SDK
* MySQL 8.0+
* Postman, Scalar or similar API client
* Docker & Docker Compose (optional)

### Installation

#### **Option 1: Local Development**

Clone the repository:

```bash
git clone <repository-url>
cd Assessment-Empleabilidad
```
Open the appsettings file to review the environment variables.

You don't need to configure a local database; the application uses a hosted MySQL instance in Aiven.
["appsetings"](appsetings-proyect.txt)


Run the application:

```bash
dotnet run
```

API available at: `https://localhost:5167`

#### **Option 2: Docker Deployment**

Build and run with Docker Compose:

```bash
docker compose up --build
```


---

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint           | Description        | Access        |
| ------ | ------------------ | ------------------ | ------------- |
| POST   | /api/Auth/login    | User login         | Public        |
| POST   | /api/Auth/register | User registration  | Public        |
| POST   | /api/Auth/refresh  | Refresh JWT tokens | Authenticated |

### User Management (Admin Only)

| Method | Endpoint                 | Description       |
| ------ | ------------------------ | ----------------- |
| GET    | /api/Users               | Get all users     |
| GET    | /api/Users/{id}          | Get user by ID    |
| GET    | /api/Users/email/{email} | Get user by email |
| POST   | /api/Users               | Create new user   |
| PUT    | /api/Users/{id}          | Update user       |
| DELETE | /api/Users/{id}          | Delete user       |

### Course Management

| Method | Endpoint                     | Description                          | Access |
|--------|------------------------------|--------------------------------------|--------|
| GET    | `/api/Course/search`         | Search & Filter (Pagination support) | Open   |
| GET    | `/api/Course/{id}`           | Get course details                   | Auth   |
| GET    | `/api/Course/{id}/summary`   | Lightweight summary (Stats)          | Auth   |
| POST   | `/api/Course`                | Create new course (Draft)            | Auth   |
| PUT    | `/api/Course/{id}`           | Update course info                   | Auth   |
| DELETE | `/api/Course/{id}`           | Soft delete course                   | Auth   |
| PATCH  | `/api/Course/{id}/publish`   | Publish (Requires active lessons)    | Auth   |
| PATCH  | `/api/Course/{id}/unpublish` | Revert to Draft                      | Auth   |

### Lesson Management

| Method | Endpoint                         | Description                    | Access |
|--------|----------------------------------|--------------------------------|--------|
| GET    | `/api/Lesson/course/{id}`        | Get all lessons for a course   | Auth   |
| POST   | `/api/Lesson`                    | Create lesson (Auto-order)     | Auth   |
| PUT    | `/api/Lesson/{id}`               | Update lesson title            | Auth   |
| DELETE | `/api/Lesson/{id}`               | Soft delete lesson             | Auth   |
| PATCH  | `/api/Lesson/{courseId}/reorder` | Complex Logic: Reorder lessons | Auth   |

*Users can only update their own profile.

---

## 🔐 Authentication & Authorization

### Default Admin User

```
Email: test@qwe.com
Username: test
Password: 123
Role: Admin
```

### JWT Token Flow

* Login returns accessToken (15 min) + refreshToken (7 days)
* Include access token as: `Authorization: Bearer {token}`
* Use refresh endpoint when expired

### Roles

* **Admin**: Full access
* **User**: Can only view info

---

## 📊 Data Models

### User

```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryDate { get; set; }
}
```

### Lesson

```csharp
public class Lesson
{
    [Key] public Guid Id { get; set; }
    [Column(TypeName = "varchar(100)")] public string Title { get; set; }
    public int Order { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    //FK
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    
}
```

### Course

```csharp
public class Course
{
    [Key] public Guid Id { get; set; }
    [Column(TypeName = "varchar(100)")] public string Title { get; set; }
    public CourseStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

}
```

---

### 🧪 Running Unit Tests
On the terminal in Folder Assessment-Empleabilidad write this comand
```bash
dotnet test
```

## 🔬 Testing the API with curl

Below are basic examples to test the API using `curl`.
Make sure the API is running locally at:

```
http://localhost:5167
```

---

## 1️⃣ Login (Get JWT Token)

```bash
curl -X POST "http://localhost:5167/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@qwe.com",
    "password": "123"
  }'
```

📌 **Response example:**

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "d8a1c7e3..."
}
```

Save the `accessToken` to use in the next requests.

---

## 2️⃣ Create a Course (Authenticated)

```bash
curl -X POST "http://localhost:5167/api/Course" \
  -H "Authorization: Bearer {accessToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Clean Architecture Fundamentals"
  }'
```

---

## 3️⃣ Search Courses (Public)

```bash
curl -X GET "http://localhost:5167/api/Course/search?page=1&pageSize=10"
```

---

## 4️⃣ Create a Lesson for a Course

```bash
curl -X POST "http://localhost:5167/api/Lesson" \
  -H "Authorization: Bearer {accessToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Introduction to Clean Architecture",
    "courseId": "COURSE_GUID_HERE"
  }'
```

📌 Lessons are automatically ordered when created.

---

## 5️⃣ Get Lessons by Course

```bash
curl -X GET "http://localhost:5167/api/Lesson/course/COURSE_GUID_HERE" \
  -H "Authorization: Bearer {accessToken}"
```

---

## 6️⃣ Publish a Course

> Requires at least one active lesson.

```bash
curl -X PATCH "http://localhost:5167/api/Course/COURSE_GUID_HERE/publish" \
  -H "Authorization: Bearer {accessToken}"
```

---

## 7️⃣ Reorder Lessons in a Course

```bash
curl -X PATCH "http://localhost:5167/api/Lesson/COURSE_GUID_HERE/reorder" \
  -H "Authorization: Bearer {accessToken}" \
  -H "Content-Type: application/json" \
  -d '[
    { "lessonId": "LESSON_GUID_1", "order": 1 },
    { "lessonId": "LESSON_GUID_2", "order": 2 }
  ]'
```

---

## 🔐 Authorization Header Reminder

All protected endpoints require the header:

```http
Authorization: Bearer {accessToken}
```



---

## 🐛 Troubleshooting

### Common Issues

**Database connection failed:**

* Ensure MySQL is running
* Check appsettings.json
* Validate credentials

**JWT token issues:**

* Access tokens last 15 minutes
* Use refresh token when expired

**403 Forbidden:**

* User lacks role permissions
* User trying to access another user's data

**409 Conflict:**

* Duplicate email, username, or DocNumber

---

## 📈 Performance Considerations

* EF Core connection pooling
* JWT validated in middleware
* Eager loading for employee->department
* Indexed fields

---

## 🔒 Security Features

* Password hashing (SHA256 + salt)
* Signed JWT tokens
* SQL injection protection
* CORS configurable
* Input validation
* Refresh token rotation

---

## 📁 Project Structure Details

### Key Files

* Program.cs
* appsettings.json
* TokenService.cs
* AppDbContext.cs
* Repository & Services

### Design Patterns

* Repository Pattern
* Service Layer
* DTO Pattern
* Dependency Injection
* Clean Architecture

---

## 📄 License

MIT License

---

## 👤 Author

**Name:** Miguel Angel Angarita
**Project:** Assessment_M6 - Courses & Lessons Management System
**Date:** January 2026

---

> "Good code is like good espresso: concentrated, powerful, and best served hot." — J.A.R.V.I.S.
