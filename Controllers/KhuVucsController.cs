using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Data;
using QUANLYCAYXANH.Models;

namespace QUANLYCAYXANH.Controllers
{
    public class KhuVucsController : Controller
    {
        private readonly AppDbContext _context;

        public KhuVucsController(AppDbContext context)
        {
            _context = context;
        }

        // Danh sách
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction(
                    "Index",
                    "Login");
            }
            var ds = _context.KhuVucs.ToList();
            return View(ds);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khuVuc = _context.KhuVucs
                .Include(x => x.CayXanhs)
                .FirstOrDefault(x => x.MaKhuVuc == id);

            if (khuVuc == null)
            {
                return NotFound();
            }

            return View(khuVuc);
        }
        // GET: KhuVucs/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        // POST: KhuVucs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KhuVuc khuVuc)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                _context.KhuVucs.Add(khuVuc);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(khuVuc);
        }
        // GET: KhuVucs/Edit/5
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var khuVuc = _context.KhuVucs.Find(id);

            if (khuVuc == null)
            {
                return NotFound();
            }

            return View(khuVuc);
        }
        // POST: KhuVucs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, KhuVuc khuVuc)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            if (id != khuVuc.MaKhuVuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.KhuVucs.Update(khuVuc);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(khuVuc);
        }
        // GET: KhuVucs/Delete/5
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var khuVuc = _context.KhuVucs.Find(id);

            if (khuVuc == null)
            {
                return NotFound();
            }

            return View(khuVuc);
        }
        // POST: KhuVucs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("VaiTro") != "Admin")
            {
                return RedirectToAction("Index");
            }
            var khuVuc = _context.KhuVucs.Find(id);

            if (khuVuc != null)
            {
                _context.KhuVucs.Remove(khuVuc);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}