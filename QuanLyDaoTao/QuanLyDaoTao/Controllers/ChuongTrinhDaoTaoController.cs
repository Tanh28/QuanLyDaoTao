using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class ChuongTrinhDaoTaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChuongTrinhDaoTaoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var chuongTrinhList = _context.ChuongTrinhDaoTao
                .Include(c => c.Khoa)
                .ToList();
            return View(chuongTrinhList);
        }

        public IActionResult Create()
        {
            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChuongTrinhDaoTao chuongTrinh)
        {
            if (_context.ChuongTrinhDaoTao.Any(c => c.MaCTDT == chuongTrinh.MaCTDT))
            {
                ModelState.AddModelError("MaCT", "Mã chương trình đào tạo này đã tồn tại");
                return View(chuongTrinh);
            }

            try
            {
                _context.ChuongTrinhDaoTao.Add(chuongTrinh);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới chương trình đào tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", chuongTrinh.MaKhoa);
            return View(chuongTrinh);
        }

        public IActionResult Edit(string id)
        {
            var chuongTrinh = _context.ChuongTrinhDaoTao
                .Include(c => c.Khoa)
                .FirstOrDefault(c => c.MaCTDT == id);

            if (chuongTrinh == null) return NotFound();

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", chuongTrinh.MaKhoa);
            return View(chuongTrinh);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("MaCT,MaKhoa,TenCT,MoTa,NgayBatDau,NgayKetThuc,TrangThai")] ChuongTrinhDaoTao chuongTrinh)
        {
            ModelState.Remove("Khoa");

            if (!string.IsNullOrEmpty(chuongTrinh.MaKhoa) && !_context.Khoa.Any(k => k.MaKhoa == chuongTrinh.MaKhoa))
            {
                ModelState.AddModelError("MaKhoa", "Mã khoa không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingChuongTrinh = await _context.ChuongTrinhDaoTao
                        .Include(ct => ct.Khoa)
                        .FirstOrDefaultAsync(ct => ct.MaCTDT == chuongTrinh.MaCTDT);

                    if (existingChuongTrinh == null)
                    {
                        ModelState.AddModelError("", $"Không tìm thấy chương trình đào tạo với MaCT = {chuongTrinh.MaCTDT}");
                        ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", chuongTrinh.MaKhoa);
                        return View(chuongTrinh);
                    }

                    existingChuongTrinh.MaCTDT = chuongTrinh.MaCTDT;
                    existingChuongTrinh.TenCTDT = chuongTrinh.TenCTDT;
                    existingChuongTrinh.NamBatDau = chuongTrinh.NamBatDau;
                    existingChuongTrinh.MaKhoa = chuongTrinh.MaKhoa;
                    existingChuongTrinh.TrangThai = chuongTrinh.TrangThai;

                    _context.Entry(existingChuongTrinh).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật chương trình đào tạo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message);
                }
            }

            ViewBag.KhoaList = new SelectList(_context.Khoa, "MaKhoa", "TenKhoa", chuongTrinh.MaKhoa);
            return View(chuongTrinh);
        }

        public IActionResult Delete(string id)
        {
            var chuongTrinh = _context.ChuongTrinhDaoTao.Find(id);
            if (chuongTrinh == null) return NotFound();
            _context.ChuongTrinhDaoTao.Remove(chuongTrinh);
            _context.SaveChanges();
            TempData["Success"] = "Xóa chương trình đào tạo thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
