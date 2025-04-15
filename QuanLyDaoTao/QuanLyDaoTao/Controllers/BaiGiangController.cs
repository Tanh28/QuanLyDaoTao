using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class BaiGiangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BaiGiangController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var baiGiangList = _context.BaiGiang
                .Include(bg => bg.MonHoc)
                .ToList();
            return View(baiGiangList);
        }

        public IActionResult BaiGiangIndex()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BaiGiang baiGiang)
        {
            if (_context.BaiGiang.Any(b => b.MaBG == baiGiang.MaBG))
            {
                ModelState.AddModelError("MaBG", "Mã bài giảng này đã tồn tại");
                ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", baiGiang.MaMH);
                return View(baiGiang);
            }
            try
            {
                _context.BaiGiang.Add(baiGiang);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới bài giảng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", baiGiang.MaMH);
            return View(baiGiang);
        }

        public IActionResult Edit(string id)
        {
            var baiGiang = _context.BaiGiang
                .Include(bg => bg.MonHoc)
                .FirstOrDefault(bg => bg.MaBG == id);

            if (baiGiang == null) return NotFound();

            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", baiGiang.MaMH);
            return View(baiGiang);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("MaBG,TieuDe,MaMH,NoiDung")] BaiGiang baiGiang)
        {
            if (id != baiGiang.MaBG)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBaiGiang = await _context.BaiGiang.FindAsync(baiGiang.MaBG);
                    if (existingBaiGiang == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy bài giảng với MaBG = {baiGiang.MaBG}");
                        ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", baiGiang.MaMH);
                        return View(baiGiang);
                    }

                    existingBaiGiang.TieuDe = baiGiang.TieuDe;
                    existingBaiGiang.MaMH = baiGiang.MaMH;
                    existingBaiGiang.NoiDung = baiGiang.NoiDung;

                    _context.Entry(existingBaiGiang).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật bài giảng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.MonHocList = new SelectList(_context.MonHoc, "MaMH", "TenMH", baiGiang.MaMH);
            return View(baiGiang);
        }

        public IActionResult Details(string id)
        {
            var baiGiang = _context.BaiGiang
                .Include(bg => bg.MonHoc)
                .FirstOrDefault(bg => bg.MaBG == id);

            if (baiGiang == null) return NotFound();

            return View(baiGiang);
        }

        public IActionResult Delete(string id)
        {
            var baiGiang = _context.BaiGiang.Find(id);
            if (baiGiang == null) return NotFound();
            _context.BaiGiang.Remove(baiGiang);
            _context.SaveChanges();
            TempData["Success"] = "Xóa bài giảng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
