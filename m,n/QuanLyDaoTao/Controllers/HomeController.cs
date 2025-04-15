using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTao.Models;
using QuanLyDaoTaoWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace QuanLyDaoTaoWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                // Không cần thêm dữ liệu cho admin
            }
            else if (roles.Contains("GiangVien"))
            {
                var giangVien = await _context.GiangVien
                    .Include(g => g.Khoa)
                    .Include(g => g.PhanCongGiangDays)
                        .ThenInclude(p => p.LopHoc)
                    .FirstOrDefaultAsync(g => g.Email == user.Email);

                if (giangVien != null)
                {
                    ViewBag.GiangVien = giangVien;
                    ViewBag.LopHocCount = giangVien.PhanCongGiangDays.Count;
                }
            }
            else if (roles.Contains("SinhVien"))
            {
                var sinhVien = await _context.SinhVien
                    .Include(s => s.Khoa)
                    .Include(s => s.DangKyLopHocs)
                        .ThenInclude(d => d.LopHoc)
                    .FirstOrDefaultAsync(s => s.Email == user.Email);

                if (sinhVien != null)
                {
                    ViewBag.SinhVien = sinhVien;
                    ViewBag.LopHocCount = sinhVien.DangKyLopHocs.Count;
                }
            }
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
