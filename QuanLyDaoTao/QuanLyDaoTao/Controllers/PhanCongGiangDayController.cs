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
    public class PhanCongGiangDayController : AdminController
    {
        public PhanCongGiangDayController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
        }

        public IActionResult Index()
        {
            var phanCongList = _context.PhanCongGiangDay
                .Include(p => p.GiangVien)
                .Include(p => p.MonHoc)
                .Include(p => p.LopHoc)
                .ToList();
            return View(phanCongList);
        }

        public IActionResult Create()
        {
            ViewBag.GiangVienList = new SelectList(_context.GiangVien, "MaGV", "HoTen");
            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH");
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PhanCongGiangDay phanCong)
        {
            if (_context.PhanCongGiangDay.Any(p => p.MaPCGD == phanCong.MaPCGD))
            {
                ModelState.AddModelError("MaPCGD", "Mã phân công này đã tồn tại");
                return View(phanCong);
            }

            try
            {
                _context.PhanCongGiangDay.Add(phanCong);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới phân công giảng dạy thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.GiangVienList = new SelectList(_context.GiangVien, "MaGV", "HoTen", phanCong.MaGV);
            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", phanCong.MaMH);
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", phanCong.MaLop);
            return View(phanCong);
        }

        public IActionResult Edit(string id)
        {
            var phanCong = _context.PhanCongGiangDay
                .Include(p => p.GiangVien)
                .Include(p => p.MonHoc)
                .Include(p => p.LopHoc)
                .FirstOrDefault(p => p.MaPCGD == id);

            if (phanCong == null) return NotFound();

            ViewBag.GiangVienList = new SelectList(_context.GiangVien, "MaGV", "HoTen", phanCong.MaGV);
            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", phanCong.MaMH);
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", phanCong.MaLop);
            return View(phanCong);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("MaPCGD,MaGV,MaMH,MaLop,HocKy,NamHoc")] PhanCongGiangDay phanCong)
        {
            ModelState.Remove("GiangVien");
            ModelState.Remove("MonHoc");
            ModelState.Remove("LopHoc");

            if (!string.IsNullOrEmpty(phanCong.MaGV) && !_context.GiangVien.Any(g => g.MaGV == phanCong.MaGV))
            {
                ModelState.AddModelError("MaGV", "Mã giảng viên không tồn tại.");
            }

            if (!string.IsNullOrEmpty(phanCong.MaMH) && !_context.MonHoc.Any(m => m.MaMH == phanCong.MaMH))
            {
                ModelState.AddModelError("MaMH", "Mã môn học không tồn tại.");
            }

            if (!string.IsNullOrEmpty(phanCong.MaLop) && !_context.LopHoc.Any(l => l.MaLop == phanCong.MaLop))
            {
                ModelState.AddModelError("MaLop", "Mã lớp học không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPhanCong = await _context.PhanCongGiangDay
                        .Include(p => p.GiangVien)
                        .Include(p => p.MonHoc)
                        .Include(p => p.LopHoc)
                        .FirstOrDefaultAsync(p => p.MaPCGD == phanCong.MaPCGD);

                    if (existingPhanCong == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy phân công với MaPCGD = {phanCong.MaPCGD}");
                        ViewBag.GiangVienList = new SelectList(_context.GiangVien, "MaGV", "HoTen", phanCong.MaGV);
                        ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", phanCong.MaMH);
                        ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", phanCong.MaLop);
                        return View(phanCong);
                    }

                    existingPhanCong.MaGV = phanCong.MaGV;
                    existingPhanCong.MaMH = phanCong.MaMH;
                    existingPhanCong.MaLop = phanCong.MaLop;
                    existingPhanCong.HocKy = phanCong.HocKy;
                    existingPhanCong.NamHoc = phanCong.NamHoc;

                    _context.Entry(existingPhanCong).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật phân công giảng dạy thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.GiangVienList = new SelectList(_context.GiangVien, "MaGV", "HoTen", phanCong.MaGV);
            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", phanCong.MaMH);
            ViewBag.LopHocList = new SelectList(_context.LopHoc, "MaLop", "TenLop", phanCong.MaLop);
            return View(phanCong);
        }

        public IActionResult Delete(string id)
        {
            var phanCong = _context.PhanCongGiangDay.Find(id);
            if (phanCong == null) return NotFound();
            _context.PhanCongGiangDay.Remove(phanCong);
            _context.SaveChanges();
            TempData["Success"] = "Xóa phân công giảng dạy thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
