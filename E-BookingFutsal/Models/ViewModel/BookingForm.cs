using System.ComponentModel.DataAnnotations;

namespace E_BookingFutsal.Models.ViewModel
{
    public class BookingForm
    {
        [Required(ErrorMessage = "Nama wajib diisi")]
        public string Nama { get; set; }
        public int StatusMember { get; set; }

        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        [Phone(ErrorMessage = "Nomor telepon tidak valid")]
        [StringLength(13, MinimumLength = 11, ErrorMessage = "NO Handphone Min 11 Angka")]
        public string NoHp { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Nama lapangan wajib diisi")]
        public int Lapangan { get; set; }

        [Required(ErrorMessage = "Tanggal booking wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime TglBooking { get; set; }

        [Required(ErrorMessage = "Jam mulai booking wajib diisi")]
        [DataType(DataType.Time)]
        public DateTime WaktuBooking { get; set; }

        [Required(ErrorMessage = "Durasi booking wajib diisi")]
        public int Durasi { get; set; }

        [Required(ErrorMessage = "Status booking wajib diisi")]
        public int Status { get; set; } 

        [Required(ErrorMessage = "Total harga wajib diisi")]
        public int TotalHarga { get; set; }
    }
}
