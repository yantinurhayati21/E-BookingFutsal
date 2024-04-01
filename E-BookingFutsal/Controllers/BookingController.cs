using E_BookingFutsal.Data;
using E_BookingFutsal.Models;
using E_BookingFutsal.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_BookingFutsal.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookingController(AppDbContext context, IWebHostEnvironment e)
        {
            _context = context;
            _env = e;
        }
        public IActionResult Index()
        {
            List<Booking> booking = _context.Bookings.ToList();
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Members = _context.Members.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.StatusMember
            }).ToList();

            ViewBag.Lapangan = _context.Lapang.Select(x => new SelectListItem
            {
                Value = x.IdLapangan.ToString(),
                Text = x.NamaLapangan
            }).ToList();

            ViewBag.Statuses = _context.Statuses.Select(x => new SelectListItem
            {
                Value = x.IdStatus.ToString(),
                Text = x.NamaStatus
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BookingForm bookingForm)
        {
            var AddMember = await _context.Members.FirstOrDefaultAsync(x => x.Id == bookingForm.StatusMember);
            var AddLapangan = await _context.Lapang.FirstOrDefaultAsync(x => x.IdLapangan == bookingForm.Lapangan);
            var AddStatus = await _context.Statuses.FirstOrDefaultAsync(x => x.IdStatus == bookingForm.Status);

            if (ModelState.IsValid)
            {
                // Cek apakah ada booking yang bertabrakan dengan jam yang sama pada lapangan yang sama
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b =>
                        //b.Lapangan.IdLapangan == bookingForm.Lapangan &&
                        b.TglBooking.Date == bookingForm.TglBooking.Date &&
                        b.WaktuBooking.TimeOfDay == bookingForm.WaktuBooking.TimeOfDay);

                if (existingBooking != null)
                {
                    // Jika ada booking yang bertabrakan, maka tolak booking baru
                    TempData["Message"] = "Maaf, booking Anda kami tolak. Silakan untuk mengganti pilihan lapangan atau waktu.";
                    return RedirectToAction("Create");
                }

                // Booking diterima
                TempData["Message"] = "Booking Anda diterima. Silakan untuk melakukan pembayaran.";

                // Buat objek Booking dari data yang diterima dari form
                var newBooking = new Booking
                {
                    Nama = bookingForm.Nama,
                    StatusMember = AddMember,
                    NoHp = bookingForm.NoHp,
                    Lapangan = AddLapangan,
                    TglBooking = bookingForm.TglBooking,
                    WaktuBooking = bookingForm.WaktuBooking,
                    Durasi = bookingForm.Durasi,
                    Status = AddStatus,
                    TotalHarga = bookingForm.TotalHarga
                };

                // Simpan booking baru ke dalam database
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                // Redirect ke halaman yang sesuai (misalnya, halaman sukses atau halaman lainnya)
                return RedirectToAction("Index", "Home");
            }
            return View(bookingForm);
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
        public async Task<IActionResult> Update([FromForm] Lapangan data, IFormFile foto)
        {
            var dataFromDb = await _context.Lapang.FirstOrDefaultAsync(x => x.IdLapangan == data.IdLapangan);

            if (dataFromDb != null)
            {
                dataFromDb.NamaLapangan = data.NamaLapangan;

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

                        dataFromDb.Photo = fileName;
                    }
                }

                // Konfirmasi perubahan ke dalam konteks basis data
                _context.Lapang.Update(dataFromDb);
                await _context.SaveChangesAsync();
                TempData["success"] = "Lapangan berhasil diperbarui.";
            }
            return RedirectToAction("Index");
        }

    }
}
