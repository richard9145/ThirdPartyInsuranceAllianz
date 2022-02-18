namespace ThirdPartyInsurance.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string? BookingRef { get; set; }
        public string? Status { get; set; }
        public double? Amount { get; set; }
        public double? AmountPaid { get; set; }
        public string? Currency { get; set; }
        public string? RawResponse { get; set; }
        public string? RawResponseVarification { get; set; }
        public string? ProcessorResponse { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;


        public string? PaymentPartner { get; set; }
        public string? PaymentStatus { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
