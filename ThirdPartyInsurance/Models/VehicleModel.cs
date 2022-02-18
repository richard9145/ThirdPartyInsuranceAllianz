using System.ComponentModel;

namespace ThirdPartyInsurance.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VehicleMake? Make { get; set; }

        [DisplayName("Make")]
        public int MakeId { get; set; }
    }
}
