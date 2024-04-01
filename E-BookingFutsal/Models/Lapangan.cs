using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_BookingFutsal.Models
{
    public class Lapangan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLapangan { get; set; }

        [Required(ErrorMessage = "Nama lapangan wajib diisi")]
        public string NamaLapangan { get; set; }

        public int HargaSewaPerJam { get; set; }

        public string Photo { get; set; }
    }

    public class LapanganForm
    {
        [Required(ErrorMessage = "Nama lapangan wajib diisi")]
        public string NamaLapangan { get; set; }
    }
}
