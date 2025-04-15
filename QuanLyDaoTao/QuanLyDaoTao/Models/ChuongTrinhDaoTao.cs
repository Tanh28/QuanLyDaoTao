using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDaoTaoWeb.Models
{
    public class ChuongTrinhDaoTao
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã chương trình")]
        public required string MaCTDT { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên chương trình")]
        public required string TenCTDT { get; set; }

        [Required]
        [Range(2000, 2030)]
        [Display(Name = "Năm bắt đầu")]
        public int NamBatDau { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã khoa")]
        public required string MaKhoa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaKhoa")]
        public required virtual Khoa Khoa { get; set; }

        public virtual required ICollection<LopHoc> LopHocs { get; set; }
    }
}
