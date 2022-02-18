namespace ThirdPartyInsurance.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public  VehicleModel? Model { get; set; }
        public int ModelId { get; set; }
        public  BodyType? BodyType { get; set; }
        public int BodyTypeId { get; set; }
    }
}
