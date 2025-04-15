using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class MonHocController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MonHocController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var monHocList = _context.MonHoc
                .Include(m => m.Khoa)
                .ToList();
            return View(monHocList);
        }

        public IActionResult Create()
        {
            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MonHoc monHoc)
        {
            if (_context.MonHoc.Any(m => m.MaMH == monHoc.MaMH))
            {
                ModelState.AddModelError("MaMH", "Mã môn học này đã tồn tại");
                ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
                return View(monHoc);
            }

            try
            {
                _context.MonHoc.Add(monHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới môn học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
            return View(monHoc);
        }

        public IActionResult Edit(string id)
        {
            var monHoc = _context.MonHoc
                .Include(m => m.Khoa)
                .FirstOrDefault(m => m.MaMH == id);

            if (monHoc == null) return NotFound();

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
            return View(monHoc);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("MaMH,MaKhoa,TenMH,SoTinChi")] MonHoc monHoc)
        {
            if (id != monHoc.MaMH)
            {
                return NotFound();
            }

            ModelState.Remove("Khoa");

            if (!string.IsNullOrEmpty(monHoc.MaKhoa) && !_context.Khoa.Any(k => k.MaKhoa == monHoc.MaKhoa))
            {
                ModelState.AddModelError("MaKhoa", "Mã khoa không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMonHoc = await _context.MonHoc
                        .Include(mh => mh.Khoa)
                        .FirstOrDefaultAsync(mh => mh.MaMH == monHoc.MaMH);

                    if (existingMonHoc == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy môn học với MaMH = {monHoc.MaMH}");
                        ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
                        return View(monHoc);
                    }

                    existingMonHoc.MaKhoa = monHoc.MaKhoa;
                    existingMonHoc.TenMH = monHoc.TenMH;
                    existingMonHoc.SoTinChi = monHoc.SoTinChi;

                    _context.Entry(existingMonHoc).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật môn học thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", monHoc.MaKhoa);
            return View(monHoc);
        }

        public IActionResult Delete(string id)
        {
            var monHoc = _context.MonHoc.Find(id);
            if (monHoc == null) return NotFound();
            _context.MonHoc.Remove(monHoc);
            _context.SaveChanges();
            TempData["Success"] = "Xóa môn học thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
