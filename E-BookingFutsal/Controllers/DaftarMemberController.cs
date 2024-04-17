using E_BookingFutsal.Data;
using E_BookingFutsal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_BookingFutsal.Controllers
{
    public class DaftarMemberController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DaftarMemberController(AppDbContext context, IWebHostEnvironment e)
        {
            _context = context;
            _env = e;
        }
     
        public IActionResult ListMember()
        {
            List<DaftarMember> members = _context.DaftarMembers.ToList();
            return View(members);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DaftarMember data, IFormFile foto)
        {

            // Validasi unik username
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

            // Simpan foto jika ada
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

            return RedirectToAction("Index","DaftarMember");
        }

        public IActionResult Detail(int id)
        {
            var members = _context.DaftarMembers.FirstOrDefault(u => u.IdMember == id);
            if (members == null)
            {
                return NotFound();
            }
            return View(members);
        }

        public IActionResult Update(int id)
        {
            var members = _context.DaftarMembers.FirstOrDefault(x => x.IdMember == id);
            return View(members);
        }

        // Method Update dalam LapanganController
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] DaftarMember data)
        {
            var dataFromDb = await _context.DaftarMembers.FirstOrDefaultAsync(x => x.IdMember == data.IdMember);

            if (dataFromDb != null)
            {
                dataFromDb.NamaMember = data.NamaMember;
                dataFromDb.Username = data.Username;
                dataFromDb.Password = data.Password;
                dataFromDb.NoHp = data.NoHp;
                dataFromDb.Email = data.Email;
                dataFromDb.Alamat = data.Alamat;
                dataFromDb.TanggalLahir = data.TanggalLahir;

                // Konfirmasi perubahan ke dalam konteks basis data
                _context.DaftarMembers.Update(dataFromDb);
                await _context.SaveChangesAsync();
                TempData["success"] = "Member berhasil diperbarui.";
            }
            return RedirectToAction("ListMember","DaftarMember");
        }

        public IActionResult Delete(int id)
        {
            var members = _context.DaftarMembers.FirstOrDefault(u => u.IdMember == id);

            if (members == null)
            {
                return NotFound();
            }
            return View(members);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var members = await _context.DaftarMembers.FindAsync(id);

            if (members == null)
            {
                return NotFound();
            }

            _context.DaftarMembers.Remove(members);
            await _context.SaveChangesAsync();
            TempData["success"] = "user deleted successfully";
            return RedirectToAction("index");
        }


        public async Task<IActionResult> DownloadFoto(int id)
        {
            var data = await _context.DaftarMembers.FirstOrDefaultAsync(x => x.IdMember == id);
            if (!string.IsNullOrEmpty(data.Foto))
            {
                var fullpath = Path.Combine(_env.WebRootPath, "members", data.Foto);
                var filebyte = System.IO.File.ReadAllBytes(fullpath);
                return File(filebyte, "application/octet-stream", data.Foto);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
