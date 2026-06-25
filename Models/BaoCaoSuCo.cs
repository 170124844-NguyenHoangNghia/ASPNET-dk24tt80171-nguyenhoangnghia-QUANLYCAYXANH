using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYCAYXANH.Models
{
    [Table("BaoCaoSuCo")]
    public class BaoCaoSuCo
    {
        [Key]
        public int MaBaoCao { get; set; }

        public int MaCay { get; set; }

        public string? NguoiBaoCao { get; set; }

        public string? SoDienThoai { get; set; }

        public string? NoiDung { get; set; }

        public DateTime NgayBaoCao { get; set; }

        public string? TrangThai { get; set; }

        [ForeignKey("MaCay")]
        public virtual CayXanh? CayXanh { get; set; }
    }
}