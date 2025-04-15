using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class TaiLieuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaiLieuController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var taiLieuList = _context.TaiLieu
                .Include(t => t.BaiGiang)
                .ToList();
            return View(taiLieuList);
        }

        public IActionResult Create()
        {
            ViewBag.BaiGiangList = new SelectList(_context.BaiGiang, "MaBG", "TieuDe");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaiLieu taiLieu)
        {
            if (_context.TaiLieu.Any(t => t.MaTL == taiLieu.MaTL))
            {
                ModelState.AddModelError("MaTL", "Mã tài liệu này đã tồn tại");
                ViewBag.BaiGiangList = new SelectList(_context.BaiGiang, "MaBG", "TieuDe", taiLieu.MaBG);
                return View(taiLieu);
            }

            try
            {
                _context.TaiLieu.Add(taiLieu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới tài liệu thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.BaiGiangList = new SelectList(_context.BaiGiang, "MaBG", "TieuDe", taiLieu.MaBG);
            return View(taiLieu);
        }

        public IActionResult Edit(string id)
        {
            var taiLieu = _context.TaiLieu
                .Include(t => t.BaiGiang)
                .FirstOrDefault(t => t.MaTL == id);

            if (taiLieu == null) return NotFound();

            ViewBag.BaiGiangList = new SelectList(_context.BaiGiang, "MaBG", "TieuDe", taiLieu.MaBG);
            return View(taiLieu);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("MaTL,MaBG,TenTL,DuongDan")] TaiLieu taiLieu)
        {
            ModelState.Remove("BaiGiang");

            if (!string.IsNullOrEmpty(taiLieu.MaBG) && !_context.BaiGiang.Any(b => b.MaBG == taiLieu.MaBG))
            {
                ModelState.AddModelError("MaBG", "Mã bài giảng không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTaiLieu = await _context.TaiLieu
                        .Include(t => t.BaiGiang)
                        .FirstOrDefaultAsync(t => t.MaTL == taiLieu.MaTL);

                    if (existingTaiLieu == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy tài liệu với MaTL = {taiLieu.MaTL}");
                        ViewBag.BaiGiangList = new SelectList(_context.BaiGiang, "MaBG", "TieuDe", taiLieu.MaBG);
                        return View(taiLieu);
                    }

                    existingTaiLieu.MaBG = taiLieu.MaBG;
                    existingTaiLieu.TenTL = taiLieu.TenTL;
                    existingTaiLieu.DuongDan = taiLieu.DuongDan;

                    _context.Entry(existingTaiLieu).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật tài liệu thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.BaiGiangList = new SelectList(_context.BaiGiang, "MaBG", "TieuDe", taiLieu.MaBG);
            return View(taiLieu);
        }

        public IActionResult Delete(string id)
        {
            var taiLieu = _context.TaiLieu.Find(id);
            if (taiLieu == null) return NotFound();
            _context.TaiLieu.Remove(taiLieu);
            _context.SaveChanges();
            TempData["Success"] = "Xóa tài liệu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
