using System.ComponentModel.DataAnnotations;

namespace QuanLyDaoTaoWeb.Models
{
    public class Khoa
    {
        public Khoa()
        {
            MonHocs = new HashSet<MonHoc>();
            GiangViens = new HashSet<GiangVien>();
            SinhViens = new HashSet<SinhVien>();
        }

        [Key]
        [StringLength(10)]
        [Display(Name = "Mã khoa")]
        public required string MaKhoa { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên khoa")]
        public required string TenKhoa { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public required string DiaChi { get; set; }

        [StringLength(15)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        [Display(Name = "Điện thoại")]
        public required string DienThoai { get; set; }

        public required virtual ICollection<MonHoc> MonHocs { get; set; }
        public required virtual ICollection<GiangVien> GiangViens { get; set; }
        public required virtual ICollection<SinhVien> SinhViens { get; set; }
    }
}
