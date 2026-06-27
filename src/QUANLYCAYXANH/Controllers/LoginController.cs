using Microsoft.AspNetCore.Mvc;
using QUANLYCAYXANH.Data;
using QUANLYCAYXANH.Models.ViewModels;

namespace QUANLYCAYXANH.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tk = _context.TaiKhoans.FirstOrDefault(x =>
                x.TenDangNhap == model.TenDangNhap &&
                x.MatKhau == model.MatKhau);

            if (tk == null)
            {
                ViewBag.Error =
                    "Sai tên đăng nhập hoặc mật khẩu";

                return View(model);
            }

            HttpContext.Session.SetString(
                "TenDangNhap",
                tk.TenDangNhap!);

            HttpContext.Session.SetString(
                "VaiTro",
                tk.VaiTro!);

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "TraCuu");
        }
    }
}