﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_BookingFutsal.Models
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama wajib diisi")]
        public string Nama { get; set; }

        [Required(ErrorMessage = "Username wajib diisi")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password wajib diisi")]
        public string Password { get; set; }

        public Roles Roles { get; set; }
        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Email tidak valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nomor telepon wajib diisi")]
        [Phone(ErrorMessage = "Nomor telepon tidak valid")]
        public string NoHp { get; set; }

        [Required(ErrorMessage = "Alamat wajib diisi")]
        public string Alamat { get; set; }
    }
}
