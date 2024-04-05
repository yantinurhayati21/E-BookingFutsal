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
            var bookings = _context.Bookings.Include(b => b.Lapangan).Include(b => b.Status).ToList();
            return View(bookings);
        }

        public IActionResult Create(int id)
        {
            var getIdLapangan = _context.Lapang.Where(l => l.IdLapangan == id).FirstOrDefault();
            return View(getIdLapangan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BookingForm data, int idLapangan, string Nama)
        {
            try
            {
                // Cari lapangan berdasarkan idLapangan yang dipilih
                var cekLapangan = await _context.Lapang.FirstOrDefaultAsync(l => l.IdLapangan == idLapangan);

                // Cek apakah ada booking yang bertabrakan dengan waktu yang sama pada lapangan yang sama
                var cekData = await _context.Bookings.FirstOrDefaultAsync(b => b.WaktuBooking == data.WaktuBooking && b.TglBooking == data.TglBooking && b.Lapangan.NamaLapangan == cekLapangan.NamaLapangan);

                // Tentukan status booking
                var statusBooking = cekData != null ? _context.Statuses.FirstOrDefault(s => s.IdStatus == 2) : _context.Statuses.FirstOrDefault(s => s.IdStatus == 1);

                // Hitung total harga berdasarkan harga lapangan dan durasi
                var hargaSewa = cekLapangan.HargaSewaPerJam;

                // Periksa jika waktu booking antara jam 19.00-00.00, tambahkan 30000 ke harga sewa per jam
                if (data.WaktuBooking.Hour >= 19 && data.WaktuBooking.Hour < 24)
                {
                    hargaSewa += 10000;
                }

                // Periksa jika waktu booking antara jam 19.00-00.00 pada hari Sabtu atau Minggu, tambahkan 20000 ke harga sewa per jam
                if ((data.WaktuBooking.DayOfWeek == DayOfWeek.Saturday || data.WaktuBooking.DayOfWeek == DayOfWeek.Sunday) && data.WaktuBooking.Hour >= 19 && data.WaktuBooking.Hour < 24)
                {
                    hargaSewa += 20000;
                }
                // Periksa jika waktu booking antara jam 8.00-18.00 pada hari Sabtu atau Minggu, tambahkan 15000 ke harga sewa per jam
                else if ((data.WaktuBooking.DayOfWeek == DayOfWeek.Saturday || data.WaktuBooking.DayOfWeek == DayOfWeek.Sunday) && data.WaktuBooking.Hour >= 8 && data.WaktuBooking.Hour < 18)
                {
                    hargaSewa += 15000;
                }

                var totalHarga = hargaSewa * data.Durasi;

                // Cari member berdasarkan nama member yang diinputkan
                var member = await _context.DaftarMembers.FirstOrDefaultAsync(m => m.NamaMember == Nama);

                // Tentukan status member dan potongan harga
                var statusMemberId = member != null ? 1 : 2; // Jika member ditemukan, status menjadi 1 (member), jika tidak, status menjadi 2 (non-member)
                var potongan = statusMemberId == 1 ? 0.1 : 0; // Jika member, berikan potongan 10%, jika bukan member, potongan 0%

                // Hitung total harga setelah potongan
                var totalHargaSetelahPotongan = totalHarga - (totalHarga * potongan);

                // Buat objek Booking baru dengan nilai yang sudah dihitung
                var newBooking = new Booking
                {
                    Nama = data.Nama,
                    StatusMember = _context.Members.FirstOrDefault(s => s.Id == statusMemberId),
                    NoHp = data.NoHp,
                    Email = data.Email,
                    Lapangan = cekLapangan,
                    TglBooking = data.TglBooking,
                    WaktuBooking = data.WaktuBooking,
                    Durasi = data.Durasi,
                    Status = statusBooking,
                    TotalHarga = (int)totalHargaSetelahPotongan // Menggunakan total harga setelah potongan
                };

                // Tambahkan booking baru ke dalam database
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                // Redirect ke halaman yang sesuai (misalnya, halaman utama)
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Tangani kesalahan
                return RedirectToAction("Error", "Home");
            }
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
            ViewBag.Members = _context.Members.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.StatusMember
            });

            ViewBag.Lapangan = _context.Lapang.Select(x => new SelectListItem
            {
                Value = x.IdLapangan.ToString(),
                Text = x.NamaLapangan
            });

            ViewBag.Status = _context.Statuses.Select(x => new SelectListItem
            {
                Value = x.IdStatus.ToString(),
                Text = x.NamaStatus
            });

            var bookingId = _context.Bookings.Include(b => b.Lapangan).Include(b => b.Status).Where(b => b.IdBooking == id).FirstOrDefault();
            return View(bookingId);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Booking data, int memberId, int lapanganId, int statusId)
        {
            data.StatusMember = _context.Members.Where(l => l.Id == memberId).FirstOrDefault();
            data.Lapangan = _context.Lapang.Where(l => l.IdLapangan == lapanganId).FirstOrDefault();
            data.Status = _context.Statuses.Where(l => l.IdStatus == statusId).FirstOrDefault();
            _context.Bookings.Update(data);
            _context.SaveChanges();
            ModelState.Clear();
            return RedirectToAction("Index", "Booking");
        }

        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.IdBooking == id);
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", "booking");
        }
    }
}
