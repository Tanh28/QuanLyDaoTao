using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyDaoTaoWeb.Models;
using Microsoft.AspNetCore.Identity;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien")]
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SinhVienController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                var sinhVienList = _context.SinhVien
                    .Include(s => s.Khoa)
                    .Include(s => s.DangKyLopHocs)
                    .Include(s => s.DanhGias)
                    .ToList();
                return View(sinhVienList);
            }
            else
            {
                var sinhVien = _context.SinhVien
                    .Include(s => s.Khoa)
                    .Include(s => s.DangKyLopHocs)
                    .Include(s => s.DanhGias)
                    .FirstOrDefault(s => s.Email == user.Email);
                if (sinhVien == null) return NotFound();
                return View("Details", sinhVien);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            if (_context.SinhVien.Any(s => s.MaSV == sinhVien.MaSV))
            {
                ModelState.AddModelError("MaSV", "Mã sinh viên này đã tồn tại");
                return View(sinhVien);
            }

            if (_context.SinhVien.Any(s => s.Email == sinhVien.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng");
                return View(sinhVien);
            }

            try
            {
                _context.SinhVien.Add(sinhVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa");
            return View(sinhVien);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var sinhVien = _context.SinhVien
                .Include(s => s.Khoa)
                .Include(s => s.DangKyLopHocs)
                .Include(s => s.DanhGias)
                .FirstOrDefault(s => s.MaSV == id);

            if (sinhVien == null) return NotFound();

            // Nếu không phải admin, chỉ cho phép sửa thông tin của chính mình
            if (!isAdmin && sinhVien.Email != user.Email)
            {
                return Forbid();
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            return View(sinhVien);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("MaSV,MaKhoa,HoTen,Email,NgaySinh")] SinhVien sinhVien)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (id != sinhVien.MaSV)
            {
                return NotFound();
            }

            // Nếu không phải admin, chỉ cho phép sửa thông tin của chính mình
            if (!isAdmin)
            {
                var existingSinhVien = await _context.SinhVien.FindAsync(id);
                if (existingSinhVien == null || existingSinhVien.Email != user.Email)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSinhVien = await _context.SinhVien.FindAsync(id);
                    if (existingSinhVien.Email != sinhVien.Email &&
                        _context.SinhVien.Any(s => s.Email == sinhVien.Email))
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng bởi sinh viên khác");
                        ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
                        return View(sinhVien);
                    }

                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin sinh viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSV))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            return View(sinhVien);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var sinhVien = await _context.SinhVien
                .Include(s => s.DangKyLopHocs)
                .Include(s => s.DanhGias)
                .FirstOrDefaultAsync(s => s.MaSV == id);

            if (sinhVien == null)
            {
                return NotFound();
            }

            if (sinhVien.DangKyLopHocs.Any() || sinhVien.DanhGias.Any())
            {
                TempData["Error"] = "Không thể xóa sinh viên này vì đã có đăng ký lớp học hoặc đánh giá!";
                return RedirectToAction(nameof(Index));
            }

            // Xóa tài khoản user tương ứng
            var user = await _userManager.FindByEmailAsync(sinhVien.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            _context.SinhVien.Remove(sinhVien);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
            return _context.SinhVien.Any(e => e.MaSV == id);
        }
    }
}
