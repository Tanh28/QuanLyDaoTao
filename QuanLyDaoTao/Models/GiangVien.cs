using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDaoTaoWeb.Models
{
    public class GiangVien
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "Mã giảng viên")]
        public required string MaGV { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public required string HoTen { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Mã khoa")]
        public required string MaKhoa { get; set; }

        [Required]
        [Display(Name = "Ngày nhận việc")]
        [DataType(DataType.Date)]
        public DateTime NgayNhanViec { get; set; }

        [ForeignKey("MaKhoa")]
        public required virtual Khoa Khoa { get; set; }

        public required virtual ICollection<PhanCongGiangDay> PhanCongGiangDays { get; set; }
    }
}
