using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDaoTaoWeb.Models
{
    public class MonHoc
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã môn học")]
        public required string MaMH { get; set; }

        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên môn học")]
        public required string TenMH { get; set; }

        [Required(ErrorMessage = "Số tín chỉ là bắt buộc")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; }

        [Required(ErrorMessage = "Khoa là bắt buộc")]
        public required string MaKhoa { get; set; }

        [ForeignKey("MaKhoa")]
        public required Khoa Khoa { get; set; }

        // Collection navigation properties
        public ICollection<BaiGiang> BaiGiangs { get; set; } = new List<BaiGiang>();
        public ICollection<PhanCongGiangDay> PhanCongGiangDays { get; set; } = new List<PhanCongGiangDay>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
