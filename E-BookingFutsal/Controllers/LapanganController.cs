using E_BookingFutsal.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using E_BookingFutsal.Models;
using Microsoft.EntityFrameworkCore;

namespace E_BookingFutsal.Controllers
{
    public class LapanganController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LapanganController(AppDbContext context, IWebHostEnvironment e)
        {
            _context = context;
            _env = e;
        }

        public IActionResult Index()
        {
            List<Lapangan> lapangan = _context.Lapang.ToList();
            return View(lapangan);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] LapanganForm data, IFormFile foto)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            var lapangan = new Lapangan()
            {
                NamaLapangan = data.NamaLapangan,
                HargaSewaPerJam = data.HargaSewaPerJam
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

                    var fileFolder = Path.Combine(_env.WebRootPath, "lapangan");

                    if (!Directory.Exists(fileFolder))
                    {
                        Directory.CreateDirectory(fileFolder);
                    }

                    var fileName = "photo_" + data.NamaLapangan + Path.GetExtension(foto.FileName);
                    var fullFilePath = Path.Combine(fileFolder, fileName);

                    using (var stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                    }

                    lapangan.Photo =fileName;
                }
            }
            _context.Lapang.Add(lapangan);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lapangan berhasil ditambahkan.";
            return RedirectToAction("Index");
        }

        public IActionResult Detail(int id)
        {
            var lapangan = _context.Lapang.FirstOrDefault(u => u.IdLapangan == id);
            if (lapangan == null)
            {
                return NotFound();
            }
            return View(lapangan);
        }

        public IActionResult Update(int id)
        {
            var lapangan = _context.Lapang.FirstOrDefault(x => x.IdLapangan == id);
            return View(lapangan);
        }

        // Method Update dalam LapanganController
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Lapangan data)
        {
            var dataFromDb = await _context.Lapang.FirstOrDefaultAsync(x => x.IdLapangan == data.IdLapangan);

            if (dataFromDb != null)
            {
                dataFromDb.NamaLapangan = data.NamaLapangan;
                dataFromDb.HargaSewaPerJam = data.HargaSewaPerJam;

                // Konfirmasi perubahan ke dalam konteks basis data
                _context.Lapang.Update(dataFromDb);
                await _context.SaveChangesAsync();
                TempData["success"] = "Lapangan berhasil diperbarui.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DownloadFoto(int id)
        {
            var data = await _context.Lapang.FirstOrDefaultAsync(x => x.IdLapangan == id);
            if (!string.IsNullOrEmpty(data.Photo))
            {
                var fullpath = Path.Combine(_env.WebRootPath, "lapangan", data.Photo);
                var filebyte = System.IO.File.ReadAllBytes(fullpath);
                return File(filebyte, "application/octet-stream", data.Photo);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
