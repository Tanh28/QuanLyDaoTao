using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.AspNetCore.Identity;


namespace QuanLyDaoTaoWeb.Controllers
{
    [Authorize(Roles = "Admin,GiangVien,SinhVien")]
    public class KhoaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public KhoaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var khoaList = _context.Khoa.ToList();
            return View(khoaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                if (_context.Khoa.Any(k => k.MaKhoa == khoa.MaKhoa))
                {
                    ModelState.AddModelError("MaKhoa", "Mã khoa đã tồn tại");
                    return View(khoa);
                }

                try
                {
                    _context.Khoa.Add(khoa);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Thêm khoa thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }

            return View(khoa);
        }

        public IActionResult Edit(string id)
        {
            var khoa = _context.Khoa.Find(id);
            if (khoa == null) return NotFound();

            return View(khoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                _context.Khoa.Update(khoa);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(khoa);
        }

        public IActionResult Delete(string id)
        {
            var khoa = _context.Khoa.Find(id);
            if (khoa == null) return NotFound();

            _context.Khoa.Remove(khoa);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
