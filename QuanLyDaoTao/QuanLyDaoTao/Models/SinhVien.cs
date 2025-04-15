using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDaoTaoWeb.Models
{
    public class SinhVien
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã sinh viên")]
        public required string MaSV { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public required string HoTen { get; set; }

        [Required]
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã khoa")]
        public required string MaKhoa { get; set; }

        [ForeignKey("MaKhoa")]
        public required virtual Khoa Khoa { get; set; }

        public required virtual ICollection<DangKyLopHoc> DangKyLopHocs { get; set; }
        public required virtual ICollection<DanhGia> DanhGias { get; set; }
    }
}
