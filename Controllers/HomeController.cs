using Microsoft.AspNetCore.Mvc;
using QUANLYCAYXANH.Models;
using System.Diagnostics;
using QUANLYCAYXANH.Data;
using QUANLYCAYXANH.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;

namespace QUANLYCAYXANH.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(
    ILogger<HomeController> logger,
    AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }
            DashboardVM vm = new DashboardVM();

            vm.TongCay =
                _context.CayXanhs.Count();

            vm.TongKhuVuc =
                _context.KhuVucs.Count();

            vm.TongChamSoc =
                _context.LichSuChamSocs.Count();

            vm.CayCanChamSoc =
                _context.CayXanhs
                .Count(x => x.TinhTrang == "Cần chăm sóc");

            vm.CaySauBenh =
                _context.CayXanhs
                .Count(x => x.TinhTrang == "Sâu bệnh");

            int quaHan = 0;

            var dsCay = _context.CayXanhs                
                .Include(x => x.LichSuChamSocs)
                .ToList();

            foreach (var cay in dsCay)
            {
                if (cay.LichSuChamSocs != null &&
                    cay.LichSuChamSocs.Any())
                {
                    var ngayGanNhat =
                        cay.LichSuChamSocs
                        .Max(x => x.NgayChamSoc);

                    int soNgay =
                        (DateTime.Now - ngayGanNhat).Days;

                    if (soNgay > 30)
                    {
                        quaHan++;
                    }
                }
            }

            vm.CayQuaHanChamSoc = quaHan;
            vm.BaoCaoChuaXuLy = _context.BaoCaoSuCos.Count(x => x.TrangThai == "Chưa xử lý");

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult CanhBao()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<CanhBaoChamSocVM> dsCanhBao =
                new List<CanhBaoChamSocVM>();

            var dsCay = _context.CayXanhs
                .Include(x => x.KhuVuc)
                .Include(x => x.LichSuChamSocs)
                .ToList();

            foreach (var cay in dsCay)
            {
                // Cảnh báo tình trạng

                if (cay.TinhTrang == "Cần chăm sóc")
                {
                    dsCanhBao.Add(
                        new CanhBaoChamSocVM
                        {
                            TenCay = cay.TenCay,
                            KhuVuc = cay.KhuVuc?.TenKhuVuc,
                            NguoiPhuTrach = cay.NguoiPhuTrach,
                            SoNgay = 0,
                            LoaiCanhBao = "Cần chăm sóc"
                        });
                }

                // Cảnh báo quá hạn chăm sóc

                if (cay.LichSuChamSocs != null &&
                    cay.LichSuChamSocs.Any())
                {
                    var ngayGanNhat =
                        cay.LichSuChamSocs
                        .Max(x => x.NgayChamSoc);

                    int soNgay =
                        (DateTime.Now - ngayGanNhat).Days;

                    if (soNgay > 30)
                    {
                        dsCanhBao.Add(
                            new CanhBaoChamSocVM
                            {
                                TenCay = cay.TenCay,
                                KhuVuc = cay.KhuVuc?.TenKhuVuc,
                                NguoiPhuTrach = cay.NguoiPhuTrach,
                                SoNgay = soNgay,
                                LoaiCanhBao = "Quá hạn chăm sóc"
                            });
                    }
                }
            }

            return View(dsCanhBao);
        }
        public IActionResult ThongKeTinhTrang()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var thongKe = _context.CayXanhs
    .GroupBy(x => x.TinhTrang)
    .Select(x => new
    {
        TinhTrang = x.Key,
        SoLuong = x.Count()
    })
    .ToList();

            var data = new[]
            {
    new
    {
        TinhTrang = "Tốt",
        SoLuong = thongKe.FirstOrDefault(x => x.TinhTrang == "Tốt")?.SoLuong ?? 0
    },
    new
    {
        TinhTrang = "Cần chăm sóc",
        SoLuong = thongKe.FirstOrDefault(x => x.TinhTrang == "Cần chăm sóc")?.SoLuong ?? 0
    },
    new
    {
        TinhTrang = "Sâu bệnh",
        SoLuong = thongKe.FirstOrDefault(x => x.TinhTrang == "Sâu bệnh")?.SoLuong ?? 0
    }
};

            ViewBag.Labels =
                System.Text.Json.JsonSerializer.Serialize(
                    data.Select(x => x.TinhTrang));

            ViewBag.Data =
                System.Text.Json.JsonSerializer.Serialize(
                    data.Select(x => x.SoLuong));

            ViewBag.CayTot =
                data.FirstOrDefault(x => x.TinhTrang == "Tốt")?.SoLuong ?? 0;

            ViewBag.CayCanChamSoc =
                data.FirstOrDefault(x => x.TinhTrang == "Cần chăm sóc")?.SoLuong ?? 0;

            ViewBag.CaySauBenh =
                data.FirstOrDefault(x => x.TinhTrang == "Sâu bệnh")?.SoLuong ?? 0;

            ViewBag.TongCay =
                data.Sum(x => x.SoLuong);

            return View();
        }
        public IActionResult ThongKeKhuVuc()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var data = _context.KhuVucs
                .Select(k => new ThongKeKhuVucVM
                {
                    TenKhuVuc = k.TenKhuVuc,
                    SoLuongCay = k.CayXanhs.Count()
                })
                .ToList();

            ViewBag.TongKhuVuc = data.Count;

            ViewBag.TongCay = data.Sum(x => x.SoLuongCay);

            ViewBag.Labels =
                System.Text.Json.JsonSerializer.Serialize(
                    data.Select(x => x.TenKhuVuc));

            ViewBag.Data =
                System.Text.Json.JsonSerializer.Serialize(
                    data.Select(x => x.SoLuongCay));

            return View(data);
        }
        public IActionResult ThongKeChamSoc()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var data = _context.LichSuChamSocs
    .GroupBy(x => x.NgayChamSoc.Month)
    .Select(x => new
    {
        ThangSo = x.Key,
        Thang = "Tháng " + x.Key,
        SoLuong = x.Count()
    })
    .OrderBy(x => x.ThangSo)
    .ToList();

            ViewBag.Labels =
                System.Text.Json.JsonSerializer.Serialize(
                    data.Select(x => x.Thang));

            ViewBag.Data =
                System.Text.Json.JsonSerializer.Serialize(
                    data.Select(x => x.SoLuong));

            ViewBag.TongChamSoc =
                _context.LichSuChamSocs.Count();

            ViewBag.SoThangCoDuLieu =
                data.Count;

            return View();
        }
        public IActionResult ExportCanhBao()
        {
            var dsCanhBao = new List<CanhBaoChamSocVM>();

            var dsCay = _context.CayXanhs
                .Include(x => x.KhuVuc)
                .Include(x => x.LichSuChamSocs)
                .ToList();

            foreach (var cay in dsCay)
            {
                if (cay.TinhTrang == "Cần chăm sóc")
                {
                    dsCanhBao.Add(new CanhBaoChamSocVM
                    {
                        TenCay = cay.TenCay,
                        KhuVuc = cay.KhuVuc?.TenKhuVuc,
                        NguoiPhuTrach = cay.NguoiPhuTrach,
                        LoaiCanhBao = "Cần chăm sóc",
                        SoNgay = 0
                    });
                }

                if (cay.TinhTrang == "Sâu bệnh")
                {
                    dsCanhBao.Add(new CanhBaoChamSocVM
                    {
                        TenCay = cay.TenCay,
                        KhuVuc = cay.KhuVuc?.TenKhuVuc,
                        NguoiPhuTrach = cay.NguoiPhuTrach,
                        LoaiCanhBao = "Sâu bệnh",
                        SoNgay = 0
                    });
                }

                if (cay.LichSuChamSocs != null &&
                    cay.LichSuChamSocs.Any())
                {
                    var ngayGanNhat =
                        cay.LichSuChamSocs.Max(x => x.NgayChamSoc);

                    int soNgay =
                        (DateTime.Now - ngayGanNhat).Days;

                    if (soNgay > 30)
                    {
                        dsCanhBao.Add(new CanhBaoChamSocVM
                        {
                            TenCay = cay.TenCay,
                            KhuVuc = cay.KhuVuc?.TenKhuVuc,
                            NguoiPhuTrach = cay.NguoiPhuTrach,
                            LoaiCanhBao = "Quá hạn chăm sóc",
                            SoNgay = soNgay
                        });
                    }
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("CanhBao");

                // TIÊU ĐỀ

                ws.Cell("A1").Value = "TRƯỜNG ĐẠI HỌC TRÀ VINH";
                ws.Range("A1:F1").Merge();

                ws.Cell("A2").Value = "DANH SÁCH CẢNH BÁO CÂY XANH";
                ws.Range("A2:F2").Merge();

                ws.Range("A3:F3").Merge();

                ws.Cell("A3").Value =
                    "Ngày xuất: " +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                ws.Cell("A3").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Right;

                ws.Range("A1:F2").Style.Font.Bold = true;
                ws.Range("A1:F2").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                // HEADER

                ws.Cell(5, 1).Value = "STT";
                ws.Cell(5, 2).Value = "Tên cây";
                ws.Cell(5, 3).Value = "Khu vực";
                ws.Cell(5, 4).Value = "Người phụ trách";
                ws.Cell(5, 5).Value = "Loại cảnh báo";
                ws.Cell(5, 6).Value = "Số ngày";

                ws.Range("A5:F5").Style.Font.Bold = true;
                ws.Range("A5:F5").Style.Fill.BackgroundColor =
                    XLColor.LightGray;

                int row = 6;
                int stt = 1;

                foreach (var item in dsCanhBao)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 2).Value = item.TenCay;
                    ws.Cell(row, 3).Value = item.KhuVuc;
                    ws.Cell(row, 4).Value = item.NguoiPhuTrach;
                    ws.Cell(row, 5).Value = item.LoaiCanhBao;
                    ws.Cell(row, 6).Value = item.SoNgay;

                    // TÔ MÀU

                    if (item.LoaiCanhBao == "Quá hạn chăm sóc")
                    {
                        ws.Range(row, 1, row, 6)
                            .Style.Fill.BackgroundColor =
                            XLColor.LightPink;
                    }

                    if (item.LoaiCanhBao == "Cần chăm sóc")
                    {
                        ws.Range(row, 1, row, 6)
                            .Style.Fill.BackgroundColor =
                            XLColor.LightYellow;
                    }

                    if (item.LoaiCanhBao == "Sâu bệnh")
                    {
                        ws.Range(row, 1, row, 6)
                            .Style.Fill.BackgroundColor =
                            XLColor.Orange;
                    }

                    row++;
                }

                ws.Cell(row + 2, 1).Value =
                    "Tổng số cảnh báo: " + dsCanhBao.Count;

                ws.Column(1).Width = 8;     // STT
                ws.Column(2).Width = 25;    // Tên cây
                ws.Column(3).Width = 20;    // Khu vực
                ws.Column(4).Width = 25;    // Người phụ trách
                ws.Column(5).Width = 25;    // Loại cảnh báo
                ws.Column(6).Width = 12;    // Số ngày

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachCanhBao.xlsx");
                }
            }
        }
    }
}
