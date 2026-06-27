using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Data;
using QUANLYCAYXANH.Models;
using ClosedXML.Excel;

namespace QUANLYCAYXANH.Controllers
{
    public class CayXanhsController : Controller
    {
        private readonly AppDbContext _context;

        public CayXanhsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(
        string? searchString,
        int? maKhuVuc,
        string? tinhTrang)
    {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }
            var ds = _context.CayXanhs
            .Include(x => x.KhuVuc)
            .AsQueryable();

        // Tìm theo tên
        if (!string.IsNullOrEmpty(searchString))
        {
            ds = ds.Where(x =>
                x.TenCay.Contains(searchString));
        }

        // Lọc theo khu vực
        if (maKhuVuc.HasValue)
        {
            ds = ds.Where(x =>
                x.MaKhuVuc == maKhuVuc);
        }

        // Lọc theo tình trạng
        if (!string.IsNullOrEmpty(tinhTrang))
        {
            ds = ds.Where(x =>
                x.TinhTrang == tinhTrang);
        }

        ViewBag.KhuVucs = _context.KhuVucs.ToList();

        ViewBag.SearchString = searchString;
        ViewBag.MaKhuVuc = maKhuVuc;
        ViewBag.TinhTrang = tinhTrang;

        return View(ds.ToList());
    }
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            ViewBag.MaKhuVuc = new SelectList(
                _context.KhuVucs,
                "MaKhuVuc",
                "TenKhuVuc");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CayXanh cayXanh)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                _context.CayXanhs.Add(cayXanh);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaKhuVuc = new SelectList(
                _context.KhuVucs,
                "MaKhuVuc",
                "TenKhuVuc",
                cayXanh.MaKhuVuc);

            return View(cayXanh);
        }
        // GET: CayXanhs/Edit/5
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var cayXanh = _context.CayXanhs.Find(id);

            if (cayXanh == null)
            {
                return NotFound();
            }

            ViewBag.MaKhuVuc = new SelectList(
                _context.KhuVucs,
                "MaKhuVuc",
                "TenKhuVuc",
                cayXanh.MaKhuVuc);

            return View(cayXanh);
        }
        // POST: CayXanhs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CayXanh cayXanh)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            if (id != cayXanh.MaCay)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.CayXanhs.Update(cayXanh);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaKhuVuc = new SelectList(
                _context.KhuVucs,
                "MaKhuVuc",
                "TenKhuVuc",
                cayXanh.MaKhuVuc);

            return View(cayXanh);
        }
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var cayXanh = _context.CayXanhs
                .FirstOrDefault(x => x.MaCay == id);

            if (cayXanh == null)
            {
                return NotFound();
            }

            return View(cayXanh);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var cayXanh = _context.CayXanhs.Find(id);

            if (cayXanh != null)
            {
                _context.CayXanhs.Remove(cayXanh);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }
            var cay = _context.CayXanhs
                .Include(x => x.KhuVuc)
                .Include(x => x.LichSuChamSocs)
                .FirstOrDefault(x => x.MaCay == id);

            if (cay == null)
            {
                return NotFound();
            }

            return View(cay);
        }
        public IActionResult ExportExcel(
    string? searchString,
    int? maKhuVuc,
    string? tinhTrang)
        {
            var ds = _context.CayXanhs
                .Include(x => x.KhuVuc)
                .AsQueryable();

            // Lọc tên cây
            if (!string.IsNullOrEmpty(searchString))
            {
                ds = ds.Where(x =>
                    x.TenCay.Contains(searchString));
            }

            // Lọc khu vực
            if (maKhuVuc.HasValue)
            {
                ds = ds.Where(x =>
                    x.MaKhuVuc == maKhuVuc);
            }

            // Lọc tình trạng
            if (!string.IsNullOrEmpty(tinhTrang))
            {
                ds = ds.Where(x =>
                    x.TinhTrang == tinhTrang);
            }

            var dsCay = ds.ToList();

            string tenKhuVuc = "Tất cả";

            if (maKhuVuc.HasValue)
            {
                tenKhuVuc = _context.KhuVucs
                    .Where(x => x.MaKhuVuc == maKhuVuc)
                    .Select(x => x.TenKhuVuc)
                    .FirstOrDefault() ?? "Tất cả";
            }

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("DanhSachCayXanh");

                // ===== TIÊU ĐỀ =====

                ws.Range("A1:G1").Merge();
                ws.Cell("A1").Value = "TRƯỜNG ĐẠI HỌC TRÀ VINH";

                ws.Range("A2:G2").Merge();
                ws.Cell("A2").Value = "DANH SÁCH CÂY XANH";

                ws.Range("A3:G3").Merge();
                ws.Cell("A3").Value =
                    "Ngày xuất: " +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                ws.Range("A4:G4").Merge();
                ws.Cell("A4").Value =
                    $"Tên cây: {(string.IsNullOrEmpty(searchString) ? "Tất cả" : searchString)}";

                ws.Range("A5:G5").Merge();
                ws.Cell("A5").Value =
                    $"Khu vực: {tenKhuVuc} | " +
                    $"Tình trạng: {(string.IsNullOrEmpty(tinhTrang) ? "Tất cả" : tinhTrang)}";

                ws.Range("A1:G5").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Range("A1:G2").Style.Font.Bold = true;
                ws.Range("A1:G2").Style.Font.FontSize = 16;

                ws.Cell("A3").Style.Font.Italic = true;
                ws.Cell("A4").Style.Font.Italic = true;
                ws.Cell("A5").Style.Font.Italic = true;

                // ===== HEADER =====

                ws.Cell(7, 1).Value = "Mã cây";
                ws.Cell(7, 2).Value = "Tên cây";
                ws.Cell(7, 3).Value = "Loài cây";
                ws.Cell(7, 4).Value = "Ngày trồng";
                ws.Cell(7, 5).Value = "Tình trạng";
                ws.Cell(7, 6).Value = "Người phụ trách";
                ws.Cell(7, 7).Value = "Khu vực";

                ws.Range("A7:G7").Style.Font.Bold = true;
                ws.Range("A7:G7").Style.Fill.BackgroundColor =
                    XLColor.LightGray;

                ws.Range("A7:G7").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                int row = 8;

                foreach (var item in dsCay)
                {
                    ws.Cell(row, 1).Value = item.MaCay;
                    ws.Cell(row, 2).Value = item.TenCay;
                    ws.Cell(row, 3).Value = item.LoaiCay;
                    ws.Cell(row, 4).Value =
                        item.NgayTrong.ToString("dd/MM/yyyy");
                    ws.Cell(row, 5).Value = item.TinhTrang;
                    ws.Cell(row, 6).Value = item.NguoiPhuTrach;
                    ws.Cell(row, 7).Value =
                        item.KhuVuc?.TenKhuVuc;

                    // Tô màu trạng thái

                    if (item.TinhTrang == "Tốt")
                    {
                        ws.Cell(row, 5).Style.Fill.BackgroundColor =
                            XLColor.LightGreen;
                    }
                    else if (item.TinhTrang == "Cần chăm sóc")
                    {
                        ws.Cell(row, 5).Style.Fill.BackgroundColor =
                            XLColor.LightYellow;
                    }
                    else if (item.TinhTrang == "Sâu bệnh")
                    {
                        ws.Cell(row, 5).Style.Fill.BackgroundColor =
                            XLColor.LightPink;
                    }

                    row++;
                }

                // ===== VIỀN =====

                ws.Range(7, 1, row - 1, 7)
                    .Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                ws.Range(7, 1, row - 1, 7)
                    .Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                // ===== ĐỘ RỘNG =====

                ws.Column(1).Width = 10;
                ws.Column(2).Width = 25;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 15;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 25;
                ws.Column(7).Width = 20;

                // ===== CĂN GIỮA =====

                ws.Column(1).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Column(4).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Column(5).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Column(7).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                // ===== TỔNG SỐ =====

                ws.Cell(row + 2, 1).Value =
                    "Tổng số cây: " + dsCay.Count;

                ws.Cell(row + 2, 1).Style.Font.Bold = true;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachCayXanh.xlsx");
                }
            }
        }
    }
}