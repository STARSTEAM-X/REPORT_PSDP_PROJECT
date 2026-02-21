namespace MyApi.Modals;

public enum ReportStatus
{
    Submitted,      // User ส่งเข้ามา
    NeedMoreInfo,   // Admin ขอข้อมูลเพิ่ม
    Accepted,       // Admin รับงาน
    InRepair,       // ส่งให้ช่างแล้ว
    ReadyToClose,   // ซ่อมเสร็จ
    Closed          // ปิดงาน
}