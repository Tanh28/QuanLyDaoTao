using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult IndexAdmin()
        {
            return View();
        }

        // Danh sách tài khoản chưa phê duyệt
        public IActionResult ApproveGiangVien()
        {
            var pendingGiangViens = _userManager.GetUsersInRoleAsync("GiangVien").Result
                .Where(u => !u.IsApproved).ToList();
            return View("ApproveGiangVien",pendingGiangViens);
        }

        // Phê duyệt tài khoản
        [HttpPost]
        public async Task<IActionResult> ApproveGiangVien(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && await _userManager.IsInRoleAsync(user, "GiangVien"))
            {
                user.IsApproved = true;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("ApproveGiangVien");
        }
    }
}
