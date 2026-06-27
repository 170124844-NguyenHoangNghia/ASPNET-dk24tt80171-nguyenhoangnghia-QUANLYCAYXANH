using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Data;
using QUANLYCAYXANH.Models;
using ClosedXML.Excel;

namespace QUANLYCAYXANH.Controllers
{
    public class LichSuChamSocsController : Controller
    {
        private readonly AppDbContext _context;

        public LichSuChamSocsController(AppDbContext context)
        {
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
            var ds = _context.LichSuChamSocs
                .Include(x => x.CayXanh)
                .ToList();

            return View(ds);
        }

        public IActionResult Create()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin"
                && vaiTro != "NhanVien")
            {
                return RedirectToAction("Index");
            }
            ViewBag.MaCay = new SelectList(
                _context.CayXanhs,
                "MaCay",
                "TenCay");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LichSuChamSoc lichSu)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin"
                && vaiTro != "NhanVien")
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                _context.LichSuChamSocs.Add(lichSu);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(lichSu);
        }
        public IActionResult Edit(int id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin"
                && vaiTro != "NhanVien")
            {
                return RedirectToAction("Index");
            }
            var lichSu = _context.LichSuChamSocs.Find(id);

            if (lichSu == null)
            {
                return NotFound();
            }

            ViewBag.MaCay = new SelectList(
                _context.CayXanhs,
                "MaCay",
                "TenCay",
                lichSu.MaCay);

            return View(lichSu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LichSuChamSoc lichSu)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin"
                && vaiTro != "NhanVien")
            {
                return RedirectToAction("Index");
            }
            if (id != lichSu.MaChamSoc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.LichSuChamSocs.Update(lichSu);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(lichSu);
        }
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var lichSu = _context.LichSuChamSocs
                .Include(x => x.CayXanh)
                .FirstOrDefault(x => x.MaChamSoc == id);

            if (lichSu == null)
            {
                return NotFound();
            }

            return View(lichSu);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var lichSu = _context.LichSuChamSocs.Find(id);

            if (lichSu != null)
            {
                _context.LichSuChamSocs.Remove(lichSu);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult ExportExcel()
        {
            var ds = _context.LichSuChamSocs
                .Include(x => x.CayXanh)
                .ThenInclude(x => x.KhuVuc)
                .OrderByDescending(x => x.NgayChamSoc)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("LichSuChamSoc");

                // TIÊU ĐỀ

                ws.Range("A1:G1").Merge();
                ws.Cell("A1").Value = "TRƯỜNG ĐẠI HỌC TRÀ VINH";

                ws.Range("A2:G2").Merge();
                ws.Cell("A2").Value = "DANH SÁCH LỊCH SỬ CHĂM SÓC CÂY XANH";

                ws.Range("A3:G3").Merge();
                ws.Cell("A3").Value =
                    "Ngày xuất: " +
                    DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                ws.Range("A1:G3").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Range("A1:G2").Style.Font.Bold = true;
                ws.Range("A1:G2").Style.Font.FontSize = 16;

                // HEADER

                ws.Cell(5, 1).Value = "STT";
                ws.Cell(5, 2).Value = "Tên cây";
                ws.Cell(5, 3).Value = "Khu vực";
                ws.Cell(5, 4).Value = "Loại chăm sóc";
                ws.Cell(5, 5).Value = "Ngày chăm sóc";
                ws.Cell(5, 6).Value = "Ghi chú";
                ws.Cell(5, 7).Value = "Người phụ trách";

                ws.Range("A5:G5").Style.Font.Bold = true;
                ws.Range("A5:G5").Style.Fill.BackgroundColor =
                    XLColor.LightGray;

                int row = 6;
                int stt = 1;

                foreach (var item in ds)
                {
                    ws.Cell(row, 1).Value = stt++;

                    ws.Cell(row, 2).Value =
                        item.CayXanh?.TenCay;

                    ws.Cell(row, 3).Value =
                        item.CayXanh?.KhuVuc?.TenKhuVuc;

                    ws.Cell(row, 4).Value =
                        item.LoaiChamSoc;

                    ws.Cell(row, 5).Value =
                        item.NgayChamSoc.ToString("dd/MM/yyyy");

                    ws.Cell(row, 6).Value =
                        item.GhiChu;

                    ws.Cell(row, 7).Value =
                        item.CayXanh?.NguoiPhuTrach;

                    row++;
                }

                // VIỀN

                ws.Range(5, 1, row - 1, 7)
                    .Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                ws.Range(5, 1, row - 1, 7)
                    .Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;

                // ĐỘ RỘNG

                ws.Column(1).Width = 8;
                ws.Column(2).Width = 25;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 15;
                ws.Column(6).Width = 35;
                ws.Column(7).Width = 25;

                // CĂN GIỮA

                ws.Column(1).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Column(3).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                ws.Column(5).Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

                // TỔNG SỐ

                ws.Cell(row + 2, 1).Value =
                    "Tổng số lượt chăm sóc: " + ds.Count;

                ws.Cell(row + 2, 1).Style.Font.Bold = true;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "LichSuChamSoc.xlsx");
                }
            }
        }
    }
}