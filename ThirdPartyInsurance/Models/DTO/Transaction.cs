namespace ThirdPartyInsurance.Models.DTO
{
    public class Transaction
    {
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public string? RegNum { get; set; }
        public int BodyType { get; set; }
        public double Premium { get; set; }
       
        public int VehicleId { get; set; }
        public int AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Id { get; set; }

        public string? BookingRef { get; set; }
    }

}
