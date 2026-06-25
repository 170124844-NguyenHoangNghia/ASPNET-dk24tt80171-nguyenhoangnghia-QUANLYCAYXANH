using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Data;

namespace QUANLYCAYXANH.Controllers
{
    public class TraCuuController : Controller
    {
        private readonly AppDbContext _context;

        public TraCuuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var ds = _context.CayXanhs
                .Include(x => x.KhuVuc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                ds = ds.Where(x =>
                    x.TenCay.Contains(searchString));
            }

            return View(ds.ToList());
        }
    }
}