using E_BookingFutsal.Models;
using Microsoft.EntityFrameworkCore;

namespace E_BookingFutsal.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Lapangan> Lapang { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<DaftarMember> DaftarMembers { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Roles> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //isi tabel Status
            modelBuilder.Entity<Status>().HasData(new Status[]
            {
                new Status
                {
                    IdStatus = 1,
                    NamaStatus = "Diterima"
                },
                new Status
                {
                    IdStatus = 2,
                    NamaStatus = "Ditolak"
                },
            });

            //isi tabel Member
            modelBuilder.Entity<Member>().HasData(new Member[]
            {
                new Member
                {
                    Id = 1,
                    StatusMember = "Member"
                },
                new Member
                {
                    Id = 2,
                    StatusMember = "Bukan Member"
                },
            });

            //isi tabel Roles
            modelBuilder.Entity<Roles>().HasData(new Roles[]
            {
                new Roles
                {
                    Id = 1,
                    Name = "Admin"
                },
                new Roles
                {
                    Id = 2,
                    Name = "Member"
                },
            });
        }
    }
}
