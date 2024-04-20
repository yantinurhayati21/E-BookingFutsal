using E_BookingFutsal.Data;
using E_BookingFutsal.Models;
using E_BookingFutsal.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public IActionResult Index()
        {
            var bookings = _context.Bookings.Include(b => b.Lapangan).Include(b => b.Status).ToList();
            return View(bookings);
        }

        public IActionResult ListBooking()
        {
            var bookings = _context.Bookings.Include(b => b.Lapangan).Include(b => b.Status).ToList();
            return View(bookings);
        }

        public IActionResult Jadwal()
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
                var cekLapangan = await _context.Lapang.FirstOrDefaultAsync(l => l.IdLapangan == idLapangan);

                var cekData = await _context.Bookings.FirstOrDefaultAsync(b => b.WaktuBooking == data.WaktuBooking && b.TglBooking == data.TglBooking && b.Lapangan.NamaLapangan == cekLapangan.NamaLapangan);

                var statusBooking = cekData != null ? _context.Statuses.FirstOrDefault(s => s.IdStatus == 2) : _context.Statuses.FirstOrDefault(s => s.IdStatus == 1);

                var hargaSewa = cekLapangan.HargaSewaPerJam;

                if (data.WaktuBooking.Hour >= 19 && data.WaktuBooking.Hour < 24)
                {
                    hargaSewa += 10000;
                }

                if ((data.WaktuBooking.DayOfWeek == DayOfWeek.Saturday || data.WaktuBooking.DayOfWeek == DayOfWeek.Sunday) && data.WaktuBooking.Hour >= 19 && data.WaktuBooking.Hour < 24)
                {
                    hargaSewa += 20000;
                }
                else if ((data.WaktuBooking.DayOfWeek == DayOfWeek.Saturday || data.WaktuBooking.DayOfWeek == DayOfWeek.Sunday) && data.WaktuBooking.Hour >= 8 && data.WaktuBooking.Hour < 18)
                {
                    hargaSewa += 15000;
                }

                var totalHarga = hargaSewa * data.Durasi;

                var member = await _context.DaftarMembers.FirstOrDefaultAsync(m => m.NamaMember == Nama);

                var statusMemberId = member != null ? 1 : 2; 
                var potongan = statusMemberId == 1 ? 0.1 : 0;

                var totalHargaSetelahPotongan = totalHarga - (totalHarga * potongan);

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
                    TotalHarga = (int)totalHargaSetelahPotongan
                };

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();
                if (cekData != null)
                {
                    TempData["Error"] = "Status booking anda ditolak, silahkan booking ulang dan ubah waktu booking atau lapangan yang anda pilih.";
                }
                else
                {
                    TempData["Success"] = "Booking lapangan berhasil.";

                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]

        public IActionResult Detail(int id)
        {
            var bookings = _context.Bookings.Include(b => b.Lapangan).Include(b => b.Status).Include(b => b.StatusMember).ToList().FirstOrDefault(b => b.IdBooking==id);
            if (bookings == null)
            {
                return NotFound();
            }
            return View(bookings);
        }

        public IActionResult DetailFromMember(int id)
        {
            var bookings = _context.Bookings.Include(b => b.Lapangan).Include(b => b.Status).Include(b => b.StatusMember).ToList().FirstOrDefault(b => b.IdBooking == id);
            if (bookings == null)
            {
                return NotFound();
            }
            return View(bookings);
        }

        [Authorize]

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

        [Authorize]
        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(x => x.IdBooking == id);
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", "booking");
        }
    }
}
