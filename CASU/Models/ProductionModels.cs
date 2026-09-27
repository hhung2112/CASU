using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CASU.Models
{
    public class DashboardKpiViewModel
    {
        public int TotalPlan { get; set; }
        public int TotalActual { get; set; }
        public double OverallDefectRate { get; set; }
        public List<TotalProductionViewModel> ProductionDetails { get; set; } = new List<TotalProductionViewModel>();
    }

    public class MachineStatusViewModel
    {
        public string? MachineCode { get; set; }
        public string? ProductCategory { get; set; }
        public int ActualQty { get; set; }
        public int TargetQty { get; set; }
        public string? Performance { get; set; }
        public int InitialQty { get; set; }
        public int DefectQty { get; set; }
        public double DefectRate { get; set; }
    }

    public class ItemsViewModel
    {
        public string? ProductName { get; set; }
        public string? ProductCategory { get; set; }
        public int ActualQty { get; set; }
        public int TargetQty { get; set; }
        public string? Performance { get; set; }
        public int InitialQty { get; set; }
        public int DefectQty { get; set; }
        public double DefectRate { get; set; }
    }

    public class EmployeeStatusViewModel
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public int ActualQty { get; set; }
        public int TargetQty { get; set; }
        public string? Performance { get; set; }
        public int InitialQty { get; set; }
        public int DefectQty { get; set; }
        public double DefectRate { get; set; }
    }

    public class TotalProductionViewModel
    {
        public string? ReportDate { get; set; }
        public string? Shift { get; set; }
        public string? ProductCategory { get; set; }
        public int ActualQty { get; set; }
        public int TargetQty { get; set; }
        public string? Performance { get; set; }
        public string? BenefitTime { get; set; }
        public int InitialQty { get; set; }
        public int DefectQty { get; set; }
        public double DefectRate { get; set; }
    }

    public class LineStatusViewModel
    {
        public string? LineCode { get; set; }
        public string? ProductCategory { get; set; }
        public int ActualQty { get; set; }
        public int TargetQty { get; set; }
        public string? Performance { get; set; }
        public int InitialQty { get; set; }
        public int DefectQty { get; set; }
        public double DefectRate { get; set; }
    }

    public class QrLogViewModel
    {
        public string? Time { get; set; }
        public string? OrderCode { get; set; }
        public string? ItemCode { get; set; }
        public string? QrCode { get; set; }
        public string? Line { get; set; }
        public string? Machine { get; set; }
        public string? Employee { get; set; }
        public string? Status { get; set; }
        public string? Defect { get; set; }
    }

    [Table("ProductionHistories")]
    public class ProductionModel
    {
        [Key]
        public int Id { get; set; }
        public string? CurrentDate { get; set; }
        public string? CurrentTime { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public string? MachineCode { get; set; }
        public string? MaLenh { get; set; }
        public string? Specification { get; set; }
        public int SanLuongDat { get; set; }
        public int ChiTieu { get; set; }
        public string? QrCode { get; set; }
        public string? Position { get; set; }
        public string? ItemCode { get; set; }
        public string? QuyCach { get; set; }
        public string? MaGai { get; set; }
        public string? NhanHieu { get; set; }
        public string? SoMaHoa { get; set; }
        public string? MaDot { get; set; }
    }

    [Table("ProductionLogs")]
    public class ProductionLogModel
    {
        [Key]
        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? MachineCode { get; set; }
        public string? Position { get; set; }
        public string? ProductCategory { get; set; }
        public string? Specification { get; set; }
        public string? TreadCode { get; set; }
        public string? Brand { get; set; }
        public string? EncryptionCode { get; set; }
        public string? DotCode { get; set; }
        public string? VskDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}