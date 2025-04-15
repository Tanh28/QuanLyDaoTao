using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDaoTaoWeb.Models
{
    public class LopHoc
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã lớp")]
        public required string MaLop { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên lớp")]
        public required string TenLop { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã chương trình đào tạo")]
        public required string MaCTDT { get; set; }

        [ForeignKey("MaCTDT")]
        public required virtual ChuongTrinhDaoTao ChuongTrinhDaoTao { get; set; }

        public required virtual ICollection<DangKyLopHoc> DangKyLopHocs { get; set; }
        public required virtual ICollection<PhanCongGiangDay> PhanCongGiangDays { get; set; }
    }
}
