using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYCAYXANH.Models
{
    [Table("LichSuChamSoc")]
    public class LichSuChamSoc
    {
        [Key]
        public int MaChamSoc { get; set; }

        public int MaCay { get; set; }

        public string? LoaiChamSoc { get; set; }

        public DateTime NgayChamSoc { get; set; }

        public string? GhiChu { get; set; }

        [ForeignKey("MaCay")]
        public virtual CayXanh? CayXanh { get; set; }
    }
}