using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class DeCuongController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeCuongController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var deCuongList = _context.DeCuong.Include(d => d.MonHoc).ToList();
            return View(deCuongList);
        }

        public IActionResult Create()
        {
            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeCuong deCuong)
        {
            if (_context.DeCuong.Any(d => d.MaDC == deCuong.MaDC))
            {
                ModelState.AddModelError("MaDC", "Mã đề cương này đã tồn tại");
                return View("DeCuong/Create", deCuong);
            }

            try
            {
                _context.DeCuong.Add(deCuong);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới đề cương thành công!";
                return RedirectToAction("DeCuongIndex");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", deCuong.MaMH);
            return View("DeCuong/Create", deCuong);
        }

        public IActionResult Edit(string id)
        {
            var deCuong = _context.DeCuong
                .Include(d => d.MonHoc)
                .FirstOrDefault(d => d.MaDC == id);

            if (deCuong == null) return NotFound();

            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", deCuong.MaMH);
            return View("DeCuong/Edit", deCuong);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("MaDC,MaMH,MoTa,MucTieu,NgayCapNhat")] DeCuong deCuong)
        {
            ModelState.Remove("MonHoc");

            if (!string.IsNullOrEmpty(deCuong.MaMH) && !_context.MonHoc.Any(m => m.MaMH == deCuong.MaMH))
            {
                ModelState.AddModelError("MaMH", "Mã môn học không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDeCuong = await _context.DeCuong
                        .Include(d => d.MonHoc)
                        .FirstOrDefaultAsync(d => d.MaDC == deCuong.MaDC);

                    if (existingDeCuong == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy đề cương với MaDC = {deCuong.MaDC}");
                        ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", deCuong.MaMH);
                        return View("DeCuong/Edit", deCuong);
                    }

                    existingDeCuong.MaMH = deCuong.MaMH;
                    existingDeCuong.MoTa = deCuong.MoTa;
                    existingDeCuong.MucTieu = deCuong.MucTieu;
                    existingDeCuong.NgayCapNhat = deCuong.NgayCapNhat;

                    _context.Entry(existingDeCuong).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật đề cương thành công!";
                    return RedirectToAction("DeCuongIndex");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", deCuong.MaMH);
            return View("DeCuong/Edit", deCuong);
        }

        public IActionResult Delete(string id)
        {
            var deCuong = _context.DeCuong.Find(id);
            if (deCuong == null) return NotFound();
            _context.DeCuong.Remove(deCuong);
            _context.SaveChanges();
            TempData["Success"] = "Xóa đề cương thành công!";
            return RedirectToAction("DeCuongIndex");
        }
    }
}
