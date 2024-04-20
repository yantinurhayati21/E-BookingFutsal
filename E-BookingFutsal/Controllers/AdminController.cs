using E_BookingFutsal.Data;
using E_BookingFutsal.Models;
using E_BookingFutsal.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_BookingFutsal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, IWebHostEnvironment e)
        {
            _context = context;
            _env = e;
        }

        public IActionResult ListAdmin()
        {
            List<Admin> admins = _context.Admins.ToList();
            return View(admins);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AdminForm data)
        {
            var AddRoles = _context.Roles.FirstOrDefault(x => x.Id == data.Role);

            if (_context.DaftarMembers.Any(u => u.Username == data.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken");
                return View(data);
            }

            var newAdmin = new Admin
            {
                Nama = data.Nama,
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                Alamat = data.Alamat,
                NoHp = data.NoHp,
                Roles = AddRoles
            };
            _context.Admins.Add(newAdmin);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Successful registration. Please login.";

            return RedirectToAction("ListAdmin", "Admin");
        }

        public IActionResult Detail(int id)
        {
            var admins = _context.Admins.Include(x => x.Roles).ToList().FirstOrDefault(x => x.Id == id);           
            if (admins == null)
            {
                return NotFound();
            }
            return View(admins);
        }

        public IActionResult Update(int id)
        {
            ViewBag.Roles = _context.Roles.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            });
            var EditAdmin = _context.Admins.Include(x => x.Roles).FirstOrDefault(x => x.Id == id);
            return View(EditAdmin);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Admin data)
        {
            var dataFromDb = await _context.Admins.FirstOrDefaultAsync(x => x.Id == data.Id);

            if (dataFromDb != null)
            {
                dataFromDb.Nama = data.Nama;
                dataFromDb.Username = data.Username;
                dataFromDb.Password = data.Password;
                dataFromDb.NoHp = data.NoHp;
                dataFromDb.Email = data.Email;
                dataFromDb.Alamat = data.Alamat;
                dataFromDb.Roles = data.Roles;
                _context.Admins.Update(dataFromDb);
                await _context.SaveChangesAsync();
                TempData["success"] = "Admin berhasil diperbarui.";
            }
            return RedirectToAction("ListAdmin", "Admin");
        }
       
        public IActionResult Delete(int id)
        {
            var admins= _context.Admins.FirstOrDefault(x => x.Id == id);
            _context.Admins.Remove(admins);
            _context.SaveChanges();
            return RedirectToAction("Index", "admin");
        }
    }
}
