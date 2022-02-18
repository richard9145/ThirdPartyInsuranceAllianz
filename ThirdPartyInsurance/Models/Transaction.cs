namespace ThirdPartyInsurance.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public string? RegNum { get; set; }
        public string? BodyType { get; set; }
        public double Premium { get; set; }

        public Vehicle Vehicle { get; set; }
        public int VehicleId { get; set; }

        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
        public string? BookingRef { get; set; }

        public bool Paid { get; set; }

    }
}
