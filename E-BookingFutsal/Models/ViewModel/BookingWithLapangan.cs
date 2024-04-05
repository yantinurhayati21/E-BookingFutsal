using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace E_BookingFutsal.Models.ViewModel
{
    public class BookingWithLapangan
    {
        public Booking Booking {  get; set; }
        public Lapangan Lapangan { get; set; }
    }
}
