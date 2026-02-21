# 📦 Maintenance Report System API

ระบบแจ้งซ่อมอุปกรณ์ (Maintenance Workflow System)  
พัฒนาโดยใช้ ASP.NET Core (.NET 10) + PostgreSQL + JWT Authentication

---

## 🚀 Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL (Supabase)
- Npgsql EF Provider
- JWT Bearer Authentication
- BCrypt Password Hashing

---

## 📥 Installation

### 1️⃣ ติดตั้ง Packages

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package BCrypt.Net-Next
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```
### 2️⃣ ติดตั้ง EF CLI Tool
```bash
dotnet tool install --global dotnet-ef
```
##  🗄 Database Setup
### Drop database in Supabase
```sql
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
```

### สร้าง Migration
```bash
dotnet ef migrations add InitialCreate
```
### อัปเดต Database
```bash
dotnet ef database update
```

## ⚙️ appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "connectionString Select type .NET in appsettings.json"
  },
  "Jwt": {
    "Key": "THIS_IS_SUPER_SECRET_KEY_123456789",
    "Issuer": "MyApi",
    "Audience": "MyApiUser",
    "ExpireMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 📊 Report Timeline (ProgressLog)
```json
[
  {
    "status": "Submitted",
    "timestamp": "2026-02-20T10:00:00Z",
    "by": "User"
  },
  {
    "status": "Inspecting",
    "timestamp": "2026-02-20T10:10:00Z",
    "by": "Admin"
  }
]
```

## 🔔 Notification System
```
_context.Notifications.Add(new Notification
{
    Title = "มีงานแจ้งซ่อมใหม่",
    Description = "มีการแจ้งซ่อมเข้ามา",
    UserId = technicianId,
    ReportId = report.ReportId
});
```

