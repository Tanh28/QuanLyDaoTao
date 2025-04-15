using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,SinhVien")]
    public class DangKyLopHocController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DangKyLopHocController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var dangKyList = _context.DangKyLopHoc
                .Include(d => d.SinhVien)
                .Include(d => d.LopHoc)
                .ToList();
            return View(dangKyList);
        }

        public IActionResult Create()
        {
            ViewBag.SinhVienList = new SelectList(_context.SinhVien, "MaSV", "HoTen");
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DangKyLopHoc dangKy)
        {
            if (_context.DangKyLopHoc.Any(d => d.MaSV == dangKy.MaSV && d.MaLopHoc == dangKy.MaLopHoc))
            {
                ModelState.AddModelError("", "Sinh viên đã đăng ký lớp học này");
                return View("DangKyLopHoc/Create", dangKy);
            }

            try
            {
                _context.DangKyLopHoc.Add(dangKy);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới đăng ký lớp học thành công!";
                return RedirectToAction("DangKyLopHocIndex");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.SinhVienList = new SelectList(_context.SinhVien, "MaSV", "HoTen", dangKy.MaSV);
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", dangKy.MaLopHoc);
            return View("DangKyLopHoc/Create", dangKy);
        }

        public IActionResult Edit(string maSV, string maLopHoc)
        {
            var dangKy = _context.DangKyLopHoc
                .Include(d => d.SinhVien)
                .Include(d => d.LopHoc)
                .FirstOrDefault(d => d.MaSV == maSV && d.MaLopHoc == maLopHoc);

            if (dangKy == null) return NotFound();

            ViewBag.SinhVienList = new SelectList(_context.SinhVien, "MaSV", "HoTen", dangKy.MaSV);
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", dangKy.MaLopHoc);
            return View("DangKyLopHoc/Edit", dangKy);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("MaSV,MaLopHoc,NgayDangKy")] DangKyLopHoc dangKy)
        {
            ModelState.Remove("SinhVien");
            ModelState.Remove("LopHoc");

            if (!string.IsNullOrEmpty(dangKy.MaSV) && !_context.SinhVien.Any(s => s.MaSV == dangKy.MaSV))
            {
                ModelState.AddModelError("MaSV", "Mã sinh viên không tồn tại.");
            }

            if (!string.IsNullOrEmpty(dangKy.MaLopHoc) && !_context.LopHoc.Any(l => l.MaLop == dangKy.MaLopHoc))
            {
                ModelState.AddModelError("MaLopHoc", "Mã lớp học không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDangKy = await _context.DangKyLopHoc
                        .Include(d => d.SinhVien)
                        .Include(d => d.LopHoc)
                        .FirstOrDefaultAsync(d => d.MaSV == dangKy.MaSV && d.MaLopHoc == dangKy.MaLopHoc);

                    if (existingDangKy == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy đăng ký với MaSV = {dangKy.MaSV} và MaLopHoc = {dangKy.MaLopHoc}");
                        ViewBag.SinhVienList = new SelectList(_context.SinhVien, "MaSV", "HoTen", dangKy.MaSV);
                        ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", dangKy.MaLopHoc);
                        return View("DangKyLopHoc/Edit", dangKy);
                    }

                    existingDangKy.NgayDangKy = dangKy.NgayDangKy;

                    _context.Entry(existingDangKy).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật đăng ký lớp học thành công!";
                    return RedirectToAction("DangKyLopHocIndex");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.SinhVienList = new SelectList(_context.SinhVien, "MaSV", "HoTen", dangKy.MaSV);
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", dangKy.MaLopHoc);
            return View("DangKyLopHoc/Edit", dangKy);
        }

        public IActionResult Delete(string maSV, string maLopHoc)
        {
            var dangKy = _context.DangKyLopHoc.Find(maSV, maLopHoc);
            if (dangKy == null) return NotFound();
            _context.DangKyLopHoc.Remove(dangKy);
            _context.SaveChanges();
            TempData["Success"] = "Xóa đăng ký lớp học thành công!";
            return RedirectToAction("DangKyLopHocIndex");
        }
    }
}
