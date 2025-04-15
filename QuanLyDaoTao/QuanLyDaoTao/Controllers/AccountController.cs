using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLyDaoTaoWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyDaoTaoWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // Sửa từ IdentityUser
        private readonly SignInManager<ApplicationUser> _signInManager; // Sửa từ IdentityUser
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    IsApproved = model.Role != "GiangVien",
                    CreatedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);

                    if (model.Role != "GiangVien" || user.IsApproved)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        
                            return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Message"] = "Tài khoản Giảng viên đã được đăng ký. Vui lòng chờ Admin phê duyệt.";
                        return RedirectToAction("Login");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.IsApproved && await _userManager.IsInRoleAsync(user, "GiangVien"))
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản Giảng viên chưa được phê duyệt.");
                    return View(model);
                }

                if (user != null)
                {
                    // Cập nhật LastLoginDate
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Đăng nhập không thành công. Vui lòng kiểm tra email và mật khẩu.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "Email này đã được sử dụng.");
                return View();
            }
            if (user == null)
            {
                ViewBag.ErrorMessage = "Không tìm thấy người dùng với email này.";
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                ViewBag.ErrorMessage = "Vai trò không tồn tại.";
                return View();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index", "Home");
        }
    }
}
