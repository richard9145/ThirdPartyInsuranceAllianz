#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using ThirdPartyInsurance.Data;
using ThirdPartyInsurance.Models;

namespace ThirdPartyInsurance.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public PaymentsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayment()
        {
            return await _context.Payment.ToListAsync();
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payment.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        // PUT: api/Payments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Payments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        ////[HttpPost]
        ////public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        ////{

        ////    string FLWSecurityKey = Startup.StaticConfig["FLWSecurityKey"].ToString();
        ////    FLWVerificationResponse FLWVerification = new FLWVerificationResponse();
        ////    var booking = await _context.Transaction.Where(c => c.BookingRef == lfwPaymentResponse.tx_ref).FirstOrDefaultAsync();

        ////    Payment payments = new Payment
        ////    {
        ////        BookingRef = booking.BookingRef,
        ////        Status = lfwPaymentResponse.status,
        ////        Amount = booking.Amount,
        ////        Currency = lfwPaymentResponse.currency,
        ////        RawResponse = JsonConvert.SerializeObject(lfwPaymentResponse),
        ////        PaymentPartner = "Flutterwave",
        ////        TransactionId = booking.Id
        ////    };


        ////    _context.Payment.Add(payment);
        ////    await _context.SaveChangesAsync();

        ////    return CreatedAtAction("GetPayment", new { id = payment.Id }, payment);
        ////}




        [HttpPost]
    

        public async Task<ActionResult<Payment>> PostFlwPayResponse(Flutterwave lfwPaymentResponse)
        {


            string FLWSecurityKey = _config["FLWSecurityKey"].ToString();
            FLWVerificationResponse FLWVerification = new FLWVerificationResponse();
            var booking = await _context.Transaction.Where(c => c.BookingRef == lfwPaymentResponse.tx_ref).FirstOrDefaultAsync();

            Payment Payment = new Payment
            {
                BookingRef = booking.BookingRef,
                Status = lfwPaymentResponse.status,
                Amount = booking.Premium,
                AmountPaid = 0,
                Currency = lfwPaymentResponse.currency,
                RawResponse = JsonConvert.SerializeObject(lfwPaymentResponse),
                PaymentPartner = "Flutterwave",
                TransactionId = booking.Id,
                PaymentStatus = "Pending",
                ProcessorResponse = "",
                RawResponseVarification = "",

            };
            _context.Payment.Add(Payment);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // _ = ErrorLogManager.LogError("Payment", ex);
            }

            //_context.SaveChanges();
            //await _context.SaveChangesAsync();

            //Validation
            RestClient restClient = new RestClient("https://api.flutterwave.com/v3/transactions/");
            RestRequest restRequest = new RestRequest("/" + lfwPaymentResponse.transaction_id + "/verify", Method.Get);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Authorization", FLWSecurityKey);
            var restResponse = await restClient.ExecuteAsync(restRequest);
            if (restResponse.IsSuccessful)
            {
                Payment.RawResponseVarification = restResponse.Content;
                try
                {
                    FLWVerification = JsonConvert.DeserializeObject<FLWVerificationResponse>(Payment.RawResponseVarification);
                }
                catch (Exception ex)
                {
                   // _ = ErrorLogManager.LogError("Payment", ex);
                }
            }
            Payment.AmountPaid = FLWVerification.data.amount;
            Payment.ProcessorResponse = FLWVerification.data.processor_response;
            Payment.PaymentStatus = FLWVerification.data.status;

            _context.Entry(Payment).State = EntityState.Modified;


            booking.Paid = true;
            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
               // _ = ErrorLogManager.LogError("Payment", ex);
            }

            return CreatedAtAction("GetPayment", new { id = Payment.Id }, Payment);
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.Id == id);
        }
    }
}
