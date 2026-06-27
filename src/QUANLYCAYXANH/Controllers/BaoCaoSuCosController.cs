using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Data;
using QUANLYCAYXANH.Models;

namespace QUANLYCAYXANH.Controllers
{
    public class BaoCaoSuCosController : Controller
    {
        private readonly AppDbContext _context;

        public BaoCaoSuCosController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var vaiTro =
        HttpContext.Session.GetString("VaiTro");

            if (vaiTro != "Admin"
                && vaiTro != "NhanVien")
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }
            var ds = _context.BaoCaoSuCos
                .Include(x => x.CayXanh)
                .OrderByDescending(x => x.NgayBaoCao)
                .ToList();

            return View(ds);
        }
        public IActionResult Create(int id)
        {
            ViewBag.MaCay = id;

            var cay = _context.CayXanhs
                .FirstOrDefault(x => x.MaCay == id);

            ViewBag.TenCay = cay?.TenCay;

            return View();
        }

        [HttpPost]
        public IActionResult Create(BaoCaoSuCo model)
        {
            model.NgayBaoCao = DateTime.Now;
            model.TrangThai = "Chưa xử lý";

            _context.BaoCaoSuCos.Add(model);
            _context.SaveChanges();

            TempData["Success"] =
                "Gửi phản ánh thành công.";

            return RedirectToAction(
                "Index",
                "TraCuu");
        }
        public IActionResult XuLy(int id)
        {
            var bc = _context.BaoCaoSuCos
                .FirstOrDefault(x => x.MaBaoCao == id);

            if (bc != null)
            {
                bc.TrangThai = "Đã xử lý";

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}