using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using CASU.Models;
using CASU.Data;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using System;

namespace CASU.Controllers
{
    public class ProductionController : Controller
    {
        private readonly string _dataFolderPath;
        private readonly AppDbContext _context;

        public ProductionController(IWebHostEnvironment env, AppDbContext context)
        {
            // Đảm bảo đường dẫn Data luôn trỏ chính xác đến thư mục Data của ứng dụng
            _dataFolderPath = Path.Combine(env.ContentRootPath, "Data");
            _context = context;
        }

        // 1. Dashboard tổng quan sản lượng
        public IActionResult TotalProduction()
        {
            var model = new DashboardKpiViewModel();
            var filePath = Path.Combine(_dataFolderPath, "Tong_San_Luong__updated.csv");

            if (!System.IO.File.Exists(filePath))
            {
                model.ProductionDetails = new List<TotalProductionViewModel> {
          new TotalProductionViewModel { ReportDate = "LỖI: Không tìm thấy file tại đường dẫn: " + filePath }
        };
                return View(model);
            }

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                int totalPlan = 0;
                int totalActual = 0;
                double totalDefectRate = 0;
                int count = 0;

                var details = new List<TotalProductionViewModel>();

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = ParseCsvLine(line);

                    if (cols.Count >= 10)
                    {
                        try
                        {
                            string reportDate = cols[0].Trim().Trim('"').Replace("\ufeff", "");

                            if (reportDate.Equals("NGÀY", StringComparison.OrdinalIgnoreCase) ||
                              reportDate.Equals("ReportDate", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            string shift = cols[1].Trim().Trim('"');
                            string category = cols[2].Trim().Trim('"');

                            int act = 0;
                            int.TryParse(cols[3].Trim().Replace("\"", "").Replace(",", ""), out act);

                            int tgt = 0;
                            int.TryParse(cols[4].Trim().Replace("\"", "").Replace(",", ""), out tgt);

                            string performance = cols[5].Trim().Replace("\"", "");
                            string benefitTime = cols[6].Trim().Replace("\"", "");

                            int ini = 0;
                            int.TryParse(cols[7].Trim().Replace("\"", "").Replace(",", ""), out ini);

                            int def = 0;
                            int.TryParse(cols[8].Trim().Replace("\"", "").Replace(",", ""), out def);

                            double rate = 0;
                            double.TryParse(cols[9].Trim().Replace("%", "").Replace("\"", "").Replace(",", "."), CultureInfo.InvariantCulture, out rate);

                            totalActual += act;
                            totalPlan += tgt;
                            totalDefectRate += rate;
                            count++;

                            details.Add(new TotalProductionViewModel
                            {
                                ReportDate = reportDate,
                                Shift = shift,
                                ProductCategory = category,
                                ActualQty = act,
                                TargetQty = tgt,
                                Performance = performance,
                                BenefitTime = benefitTime,
                                InitialQty = ini,
                                DefectQty = def,
                                DefectRate = rate
                            });
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                model.TotalPlan = totalPlan;
                model.TotalActual = totalActual;
                model.OverallDefectRate = count > 0 ? System.Math.Round(totalDefectRate / count, 2) : 0;
                model.ProductionDetails = details;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("--> LỖI KHI ĐỌC FILE TOTAL PRODUCTION: " + ex.Message);
            }

            return View(model);
        }

        // 2. Quản lý dây chuyền và máy (Machines)
        public IActionResult Machines()
        {
            var list = new List<MachineStatusViewModel>();
            var filePath = Path.Combine(_dataFolderPath, "SL_Theo_May__updated.csv");

            if (!System.IO.File.Exists(filePath))
            {
                System.Console.WriteLine("--> CẢNH BÁO: Không tìm thấy file CSV tại: " + filePath);
                return View(list); // Trả về danh sách rỗng để không bị crash trang web
            }

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = ParseCsvLine(line);
                    if (cols.Count >= 7)
                    {
                        try
                        {
                            string rawCol0 = cols[0].Trim().Trim('"');
                            string machineCode = rawCol0;
                            var parts = rawCol0.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 2)
                            {
                                machineCode = parts[1];
                            }

                            string productCategory = cols[1].Trim().Trim('"');

                            int act = 0;
                            int.TryParse(cols[2].Trim().Replace("\"", "").Replace(",", ""), out act);

                            int tgt = 0;
                            int.TryParse(cols[3].Trim().Replace("\"", "").Replace(",", ""), out tgt);

                            string performance = tgt > 0 ? $"{Math.Round((double)act / tgt * 100, 2)}%" : "0%";

                            int ini = 0;
                            int.TryParse(cols[4].Trim().Replace("\"", "").Replace(",", ""), out ini);

                            int def = 0;
                            int.TryParse(cols[5].Trim().Replace("\"", "").Replace(",", ""), out def);

                            double rate = 0;
                            double.TryParse(cols[6].Trim().Replace("%", "").Replace("\"", "").Replace(",", "."), CultureInfo.InvariantCulture, out rate);

                            list.Add(new MachineStatusViewModel
                            {
                                MachineCode = machineCode,
                                ProductCategory = productCategory,
                                ActualQty = act,
                                TargetQty = tgt,
                                Performance = performance,
                                InitialQty = ini,
                                DefectQty = def,
                                DefectRate = rate
                            });
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("--> LỖI KHI ĐỌC FILE MACHINES: " + ex.Message);
            }

            return View(list);
        }

        // 3. Quản lý sản phẩm/Item
        public IActionResult Item()
        {
            var list = new List<ItemsViewModel>();
            var filePath = Path.Combine(_dataFolderPath, "San_Luong_Theo_Item__updated.csv");

            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = ParseCsvLine(line);

                    if (cols.Count >= 8)
                    {
                        list.Add(new ItemsViewModel
                        {
                            ProductName = cols[0].Trim().Trim('"'),
                            ProductCategory = cols[1].Trim().Trim('"'),
                            ActualQty = int.TryParse(cols[2].Trim().Replace("\"", "").Replace(",", ""), out var act) ? act : 0,
                            TargetQty = int.TryParse(cols[3].Trim().Replace("\"", "").Replace(",", ""), out var tgt) ? tgt : 0,
                            Performance = cols[4].Trim().Trim('"'),
                            InitialQty = int.TryParse(cols[5].Trim().Replace("\"", "").Replace(",", ""), out var ini) ? ini : 0,
                            DefectQty = int.TryParse(cols[6].Trim().Replace("\"", "").Replace(",", ""), out var def) ? def : 0,
                            DefectRate = double.TryParse(cols[7].Trim().Replace("%", "").Replace("\"", "").Replace(",", "."), CultureInfo.InvariantCulture, out var rate) ? rate : 0
                        });
                    }
                }
            }
            return View(list);
        }

        // 4. Năng suất nhân viên
        public IActionResult Employees()
        {
            var list = new List<EmployeeStatusViewModel>();
            var filePath = Path.Combine(_dataFolderPath, "SL_Theo_NhanVien__updated.csv");

            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = ParseCsvLine(line);

                    if (cols.Count >= 8)
                    {
                        string empName = cols[0].Trim().Trim('"');
                        if (empName.Equals("TÊN NHÂN VIÊN", StringComparison.OrdinalIgnoreCase)) continue;

                        list.Add(new EmployeeStatusViewModel
                        {
                            EmployeeName = empName,
                            EmployeeCode = cols[1].Trim().Trim('"'),
                            ActualQty = int.TryParse(cols[2].Trim().Replace("\"", "").Replace(",", ""), out var act) ? act : 0,
                            TargetQty = int.TryParse(cols[3].Trim().Replace("\"", "").Replace(",", ""), out var tgt) ? tgt : 0,
                            Performance = cols[4].Trim().Trim('"'),
                            InitialQty = int.TryParse(cols[5].Trim().Replace("\"", ""), out var ini) ? ini : 0,
                            DefectQty = int.TryParse(cols[6].Trim().Replace("\"", "").Replace(",", ""), out var def) ? def : 0,
                            DefectRate = double.TryParse(cols[7].Trim().Replace("%", "").Replace("\"", "").Replace(",", "."), CultureInfo.InvariantCulture, out var rate) ? rate : 0
                        });
                    }
                }
            }
            return View(list);
        }

        // 5. Quản lý theo Dây Chuyền
        public IActionResult Lines()
        {
            var list = new List<LineStatusViewModel>();
            var filePath = Path.Combine(_dataFolderPath, "SL_Theo_DayChuyen__updated.csv");

            if (System.IO.File.Exists(filePath))
            {
                var lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = ParseCsvLine(line);

                    if (cols.Count >= 8)
                    {
                        list.Add(new LineStatusViewModel
                        {
                            LineCode = cols[0].Trim().Trim('"'),
                            ProductCategory = cols[1].Trim().Trim('"'),
                            ActualQty = int.TryParse(cols[2].Trim().Replace("\"", "").Replace(",", ""), out var act) ? act : 0,
                            TargetQty = int.TryParse(cols[3].Trim().Replace("\"", "").Replace(",", ""), out var tgt) ? tgt : 0,
                            Performance = cols[4].Trim().Replace("\"", ""),
                            InitialQty = int.TryParse(cols[5].Trim().Replace("\"", "").Replace(",", ""), out var ini) ? ini : 0,
                            DefectQty = int.TryParse(cols[6].Trim().Replace("\"", "").Replace(",", ""), out var def) ? def : 0,
                            DefectRate = double.TryParse(cols[7].Trim().Replace("%", "").Replace("\"", "").Replace(",", "."), CultureInfo.InvariantCulture, out var rate) ? rate : 0
                        });
                    }
                }
            }
            return View(list);
        }

        // 6. Nhật ký quét mã QR
        public IActionResult QrLogs()
        {
            var list = new List<QrLogViewModel>();
            var filePath = Path.Combine(_dataFolderPath, "NhatKy_QR_.csv");

            if (System.IO.File.Exists(filePath))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var fileContent = System.IO.File.ReadAllText(filePath, Encoding.GetEncoding("windows-1258"));

                var rows = ParseCsvRows(fileContent);

                for (int i = 1; i < rows.Count; i++)
                {
                    var cols = rows[i];
                    if (cols.Count >= 9)
                    {
                        list.Add(new QrLogViewModel
                        {
                            Time = cols[0].Trim().Replace("\r", "").Replace("\n", " "),
                            OrderCode = cols[1].Trim(),
                            ItemCode = cols[2].Trim(),
                            QrCode = cols[3].Trim(),
                            Line = cols[4].Trim(),
                            Machine = cols[5].Trim(),
                            Employee = cols[6].Trim(),
                            Status = cols[7].Trim(),
                            Defect = cols[8].Trim()
                        });
                    }
                }
            }
            return View(list);
        }

        // 7. Giao diện form nhập liệu (GET)
        [HttpGet]
        public IActionResult InputProduction()
        {
            return View();
        }

        // Xử lý lưu dữ liệu nhập liệu vào Database (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveProduction(IFormCollection form)
        {
            try
            {
                string employeeName = form["employeeName"];
                string employeeCode = form["employeeCode"];
                string machineCode = form["machineCode"];
                string position = form["position"];
                string itemCode = form["productCategory"];
                string quyCach = form["specification"];
                string maGai = form["treadCode"];
                string nhanHieu = form["brand"];
                string soMaHoa = form["encryptionCode"];
                string maDot = form["dotCode"];
                string ngayVsk = form["vskDate"];

                string currentDate = string.IsNullOrEmpty(ngayVsk) ? DateTime.Now.ToString("dd/MM/yyyy") : ngayVsk;

                var productionLog = new ProductionLogModel
                {
                    EmployeeName = employeeName,
                    EmployeeCode = employeeCode,
                    MachineCode = machineCode,
                    Position = position,
                    ProductCategory = itemCode,
                    Specification = quyCach,
                    TreadCode = maGai,
                    Brand = nhanHieu,
                    EncryptionCode = soMaHoa,
                    DotCode = maDot,
                    VskDate = currentDate,
                    CreatedAt = DateTime.Now
                };

                _context.ProductionLogs.Add(productionLog);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Lưu mẻ sản xuất vào Database thành công!";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu Database: " + ex.Message;
                return RedirectToAction("InputProduction");
            }
        }

        // 8. Xem lịch sử nhập liệu từ Database
        [HttpGet]
        public IActionResult History()
        {
            var listProduction = _context.ProductionLogs
              .OrderByDescending(x => x.Id)
              .ToList();

            return View(listProduction);
        }

        // --- CÁC HÀM HỖ TRỢ PHÂN TÍCH CSV (HELPER METHODS) ---
        private List<List<string>> ParseCsvRows(string csvText)
        {
            var rows = new List<List<string>>();
            var currentRow = new List<string>();
            var currentField = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvText.Length; i++)
            {
                char c = csvText[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csvText.Length && csvText[i + 1] == '"')
                        {
                            currentField.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        currentRow.Add(currentField.ToString());
                        currentField.Clear();
                    }
                    else if (c == '\r')
                    {
                        // Bỏ qua carriage return
                    }
                    else if (c == '\n')
                    {
                        currentRow.Add(currentField.ToString());
                        currentField.Clear();
                        rows.Add(currentRow);
                        currentRow = new List<string>();
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
            }

            if (currentField.Length > 0 || currentRow.Count > 0)
            {
                currentRow.Add(currentField.ToString());
                rows.Add(currentRow);
            }

            return rows;
        }

        private List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var currentField = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    currentField.Append(c);
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
            result.Add(currentField.ToString());
            return result;
        }
    }
}