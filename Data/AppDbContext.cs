using Microsoft.EntityFrameworkCore;
using QUANLYCAYXANH.Models;

namespace QUANLYCAYXANH.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<KhuVuc> KhuVucs { get; set; }

        public DbSet<CayXanh> CayXanhs { get; set; }

        public DbSet<LichSuChamSoc> LichSuChamSocs { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<BaoCaoSuCo> BaoCaoSuCos { get; set; }
    }
}