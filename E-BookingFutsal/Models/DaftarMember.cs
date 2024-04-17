using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_BookingFutsal.Models
{
    public class DaftarMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMember { get; set; }

        [Required(ErrorMessage = "Nama member wajib diisi")]
        public string NamaMember { get; set; }

        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        [Phone(ErrorMessage = "Nomor telepon tidak valid")]
        public string NoHp { get; set; }

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Email tidak valid")]
        public string Email { get; set; }

        public string Alamat { get; set; }

        public DateTime TanggalLahir { get; set; }
        public string Foto { get; set; }
    }

    public class DaftarMemberForm
    {
        [Required(ErrorMessage = "Nama member wajib diisi")]
        public string NamaMember { get; set; }

        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        [Phone(ErrorMessage = "Nomor telepon tidak valid")]
        public string NoHp { get; set; }

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Email tidak valid")]
        public string Email { get; set; }

        public string Alamat { get; set; }

        public DateTime TanggalLahir  { get; set; }
    }
}
