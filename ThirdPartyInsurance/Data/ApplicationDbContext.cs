using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThirdPartyInsurance.Models;

namespace ThirdPartyInsurance.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<VehicleMake> VehicleMakes { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ThirdPartyInsurance.Models.Transaction> Transaction { get; set; }
        public DbSet<ThirdPartyInsurance.Models.AppUser> AppUser { get; set; }
        public DbSet<ThirdPartyInsurance.Models.Payment> Payment { get; set; }
    }
}