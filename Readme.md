dotnet add package Microsoft.EntityFrameworkCore 
dotnet add package Microsoft.EntityFrameworkCore.Design 
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet tool install --global dotnet-ef


dotnet ef migrations add InitialCreate 
dotnet ef database update 


dotnet add package BCrypt.Net-Next
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer


ตัวอย่างการสร้าง Notification อัตโนมัติ
_context.Notifications.Add(new Notification
{
    Title = "มีงานแจ้งซ่อมใหม่",
    Description = "มีการแจ้งซ่อมเข้ามา",
    UserId = technicianId,
    ReportId = report.ReportId
});

ดึง Notification
GET /api/notification
Authorization: Bearer TOKEN

GET /api/notification/unread-count

PUT /api/notification/5/read