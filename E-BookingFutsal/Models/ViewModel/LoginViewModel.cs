using System.ComponentModel.DataAnnotations;

namespace E_BookingFutsal.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password wajib diisi")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Roles { get; set; }
    }
}
