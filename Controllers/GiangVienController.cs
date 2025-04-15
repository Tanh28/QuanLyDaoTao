using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDaoTaoWeb.Models;
using Microsoft.AspNetCore.Identity;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class GiangVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GiangVienController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Nếu không lấy được user (chưa đăng nhập hoặc token hỏng), trả về trang AccessDenied hoặc chuyển hướng
                return RedirectToAction("AccessDenied", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");


            var giangVienList = _context.GiangVien
                .Include(g => g.Khoa)
                .Include(g => g.PhanCongGiangDays)
                .ToList();
            return View(giangVienList);

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(GiangVien giangVien)
        {
            if (_context.GiangVien.Any(g => g.MaGV == giangVien.MaGV))
            {
                ModelState.AddModelError("MaGV", "Mã giảng viên này đã tồn tại");
                return View(giangVien);
            }

            if (_context.GiangVien.Any(g => g.Email == giangVien.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng");
                return View(giangVien);
            }

            try
            {
                _context.GiangVien.Add(giangVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới giảng viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa");
            return View(giangVien);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var giangVien = _context.GiangVien
                .Include(g => g.Khoa)
                .Include(g => g.PhanCongGiangDays)
                .FirstOrDefault(g => g.MaGV == id);

            if (giangVien == null) return NotFound();

            // Nếu không phải admin, chỉ cho phép sửa thông tin của chính mình
            if (!isAdmin && giangVien.Email != user.Email)
            {
                return Forbid();
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
            return View(giangVien);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("MaGV,MaKhoa,HoTen,Email,NgayNhanViec")] GiangVien giangVien)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (id != giangVien.MaGV)
            {
                return NotFound();
            }

            // Nếu không phải admin, chỉ cho phép sửa thông tin của chính mình
            if (!isAdmin)
            {
                var existingGiangVien = await _context.GiangVien.FindAsync(id);
                if (existingGiangVien == null || existingGiangVien.Email != user.Email)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingGiangVien = await _context.GiangVien.FindAsync(id);
                    if (existingGiangVien.Email != giangVien.Email &&
                        _context.GiangVien.Any(g => g.Email == giangVien.Email))
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi giảng viên khác");
                        ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
                        return View(giangVien);
                    }

                    _context.Update(giangVien);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin giảng viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiangVienExists(giangVien.MaGV))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", giangVien.MaKhoa);
            return View(giangVien);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var giangVien = await _context.GiangVien
                .Include(g => g.PhanCongGiangDays)
                .FirstOrDefaultAsync(g => g.MaGV == id);

            if (giangVien == null)
            {
                return NotFound();
            }

            if (giangVien.PhanCongGiangDays.Any())
            {
                TempData["Error"] = "Không thể xóa giảng viên này vì đã có phân công giảng dạy!";
                return RedirectToAction(nameof(Index));
            }

            // Xóa tài khoản user tương ứng
            var user = await _userManager.FindByEmailAsync(giangVien.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            _context.GiangVien.Remove(giangVien);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa giảng viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool GiangVienExists(string id)
        {
            return _context.GiangVien.Any(e => e.MaGV == id);
        }
    }
}
