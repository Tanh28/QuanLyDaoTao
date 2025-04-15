using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDaoTaoWeb.Models
{
    public class PhanCongGiangDay
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã phân công")]
        public required string MaPCGD { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã giảng viên")]
        public required string MaGV { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã môn học")]
        public required string MaMH { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã lớp")]
        public required string MaLop { get; set; }

        [Required]
        [Range(1, 3)]
        [Display(Name = "Học kỳ")]
        public int HocKy { get; set; }

        [Required]
        [Range(2000, 2026)]
        [Display(Name = "Năm học")]
        public int NamHoc { get; set; }

        [ForeignKey("MaGV")]
        public required virtual GiangVien GiangVien { get; set; }

        [ForeignKey("MaMH")]
        public required virtual MonHoc MonHoc { get; set; }

        [ForeignKey("MaLop")]
        public required virtual LopHoc LopHoc { get; set; }
    }
}
