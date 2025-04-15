using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace QuanLyDaoTaoWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Trạng thái phê duyệt")]
        public bool IsApproved { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Lần đăng nhập cuối")]
        public DateTime? LastLoginDate { get; set; }
    }
}