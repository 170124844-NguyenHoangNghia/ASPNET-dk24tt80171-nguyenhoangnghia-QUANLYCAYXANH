using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Data;

namespace QUANLYCAYXANH.Controllers
{
    public class BanDoController : Controller
    {
        private readonly AppDbContext _context;

        public BanDoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {         
            var ds = _context.CayXanhs
                .Include(x => x.KhuVuc)
                .ToList();
            ViewBag.DaDangNhap =
    HttpContext.Session.GetString("TenDangNhap") != null;
            return View(ds);
        }
    }
}