using E_BookingFutsal.Data;
using E_BookingFutsal.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using E_BookingFutsal.Models;
using Microsoft.Extensions.Hosting;

namespace E_BookingFutsal.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(AppDbContext context, IWebHostEnvironment e)
        {
            _context = context;
            _env = e;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            ViewBag.Roles = _context.Roles.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel data)
        {
            if (!IsRoleValid(data.Roles, data.Username, data.Password))
            {
                TempData["ErrorMessage"] = "The selected role is incorrect for this user";
                return RedirectToAction(nameof(Login));
            }

            var admin = _context.Admins.FirstOrDefault(x => x.Username == data.Username && x.Password == data.Password);
            var member = _context.DaftarMembers.FirstOrDefault(x => x.Username == data.Username && x.Password == data.Password);

            if (admin == null && member == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password";
                return RedirectToAction(nameof(Login));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, data.Username)
            };

            if (admin != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else if (member != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Member"));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["Success"] = "Login successful";

            if (admin != null)
            {
                return Redirect("/Admin/Index");
            }
            else if (member != null)
            {
                return Redirect("/DaftarMember/Index");
            }
            return Redirect("/SignIn");
        }

        private bool IsRoleValid(int selectedRole, string username, string password)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Username == username && x.Password == password);
            var member = _context.DaftarMembers.FirstOrDefault(x => x.Username == username && x.Password == password);

            if (admin != null && selectedRole != 1)
            {
                return false;
            }
            else if (member != null && selectedRole != 2)
            {
                return false;
            }

            return true;
        }

        public async Task<IActionResult> RegisterMember()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterMember(DaftarMember data, [FromForm] IFormFile foto)
        {
            if (_context.DaftarMembers.Any(u => u.Username == data.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(data);
            }

            var newMember = new DaftarMember
            {
                NamaMember = data.NamaMember,
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                Alamat = data.Alamat,
                NoHp = data.NoHp,
                TanggalLahir = data.TanggalLahir

            };

            if (foto != null)
            {
                if (foto.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExt = Path.GetExtension(foto.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExt))
                    {
                        ModelState.AddModelError("Foto", "File type is not allowed. Please upload a JPG or PNG file.");
                        return View(data);
                    }

                    var fileFolder = Path.Combine(_env.WebRootPath, "members");
                    if (!Directory.Exists(fileFolder))
                    {
                        Directory.CreateDirectory(fileFolder);
                    }

                    var fileName = Guid.NewGuid().ToString() + "_" + foto.FileName;
                    var filePath = Path.Combine(fileFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    newMember.Foto = fileName;
                }
            }

            _context.DaftarMembers.Add(newMember);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Successful registration. Please login.";

            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(Admin data)
        {
            if (_context.Admins.Any(u => u.Username == data.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(data);
            }

            var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin");

            if (adminRole == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var newAdmin = new Admin
            {
                Nama = data.Nama,
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                Alamat = data.Alamat,
                NoHp = data.NoHp, 
                Roles = adminRole
            };
            _context.Admins.Add(newAdmin);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Successful registration. Please login.";

            return RedirectToAction("Login", "Account");
        }

    }
}

