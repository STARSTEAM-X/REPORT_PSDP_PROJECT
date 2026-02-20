namespace MyApi.Modals;

public enum ReportStatus
{
    Submitted, // ผู้ใช้แจ้งซ่อมเข้ามา
    Inspecting, // เจ้าหน้าที่กำลังตรวจสอบ
    InRepair, // ช่างกำลังซ่อม  
    ReadyToClose, // ช่างแจ้งว่าซ่อมเสร็จแล้ว รอ Admin ปิดงาน
    Closed     // Admin ปิดงานจริง
}