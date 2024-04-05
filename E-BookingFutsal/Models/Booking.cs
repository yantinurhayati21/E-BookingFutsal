using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_BookingFutsal.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBooking { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi")]
        public string Nama { get; set; }
        public Member StatusMember { get; set; }

        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        [Phone(ErrorMessage = "Nomor telepon tidak valid")]
        [StringLength(13, MinimumLength = 11, ErrorMessage = "NO Handphone Min 11 Angka")]
        public string NoHp { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Nama lapangan wajib diisi")]
        public Lapangan Lapangan { get; set; }

        [Required(ErrorMessage = "Tanggal booking wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime TglBooking { get; set; }

        [Required(ErrorMessage = "Jam mulai booking wajib diisi")]
        [DataType(DataType.Time)]
        public DateTime WaktuBooking { get; set; }

        [Required(ErrorMessage = "Durasi booking wajib diisi")]
        public int Durasi { get; set; }

        [Required(ErrorMessage = "Status booking wajib diisi")]
        public Status Status { get; set; }

        [Required(ErrorMessage = "Total harga wajib diisi")]
        public int TotalHarga { get; set; }
    }
}
