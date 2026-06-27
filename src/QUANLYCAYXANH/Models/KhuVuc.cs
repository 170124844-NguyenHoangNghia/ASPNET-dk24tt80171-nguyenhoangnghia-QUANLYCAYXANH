using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYCAYXANH.Models
{
    [Table("KhuVuc")]
    public class KhuVuc
    {
        [Key]
        public int MaKhuVuc { get; set; }

        public string? TenKhuVuc { get; set; }

        public string? MoTa { get; set; }

        public virtual ICollection<CayXanh>? CayXanhs { get; set; }
    }
}