using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYCAYXANH.Models
{
    [Table("CayXanh")]
    public class CayXanh
    {
        [Key]
        public int MaCay { get; set; }

        public string? TenCay { get; set; }

        public string? LoaiCay { get; set; }

        public DateTime NgayTrong { get; set; }

        public string? TinhTrang { get; set; }

        public string? NguoiPhuTrach { get; set; }

        public int MaKhuVuc { get; set; }

        [ForeignKey("MaKhuVuc")]
        public virtual KhuVuc? KhuVuc { get; set; }
        public virtual ICollection<LichSuChamSoc>?
    LichSuChamSocs
        { get; set; }
        public double? ViDo { get; set; }
        public double? KinhDo { get; set; }
    }
}