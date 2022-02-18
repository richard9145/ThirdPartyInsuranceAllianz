#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThirdPartyInsurance.Data;
using ThirdPartyInsurance.Models;

namespace ThirdPartyInsurance.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transaction.Include(t => t.AppUser).Include(t => t.Vehicle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.AppUser)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["BodyType"] = new SelectList(_context.BodyTypes, "Id", "Name");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Include(c => c.Model.Make).Include(c => c.Model).Select(c => new { Id = c.Id, Name = c.Model.Make.Name + " " + c.Model.Name }), "Id", "Name");

            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.DTO.Transaction transaction)
        {
            //if (ModelState.IsValid)
            //{
                AppUser User = new AppUser {
                    DateOfBirth = transaction.DateOfBirth, 
                    Email = transaction.Email, 
                    FirstName = transaction.FirstName, 
                    LastName = transaction.LastName, 
                    PhoneNumber = transaction.PhoneNumber 
                };
                _context.AppUser.Add(User);
                await _context.SaveChangesAsync();

                Vehicle myVehicle = _context.Vehicles.Where(c => c.Id == transaction.VehicleId).Include(c => c.Model.Make).Include(c => c.BodyType).FirstOrDefault();
                
                BodyType myBodyType = _context.BodyTypes.Where(b => b.Id == transaction.BodyType).FirstOrDefault();

                Transaction trans = new Transaction
                { 
                    VehicleMake = myVehicle.Model.Make.Name, 
                    VehicleModel = myVehicle.Model.Name, 
                    BodyType = myBodyType.Name, 
                    Premium = myBodyType.Premium, 
                    RegNum = transaction.RegNum, 
                    AppUserId = User.Id,
                    VehicleId = transaction.VehicleId,
                    BookingRef = GenerateRandomCode(8)
                };

                transaction.BookingRef = trans.BookingRef;
                _context.Add(trans);
                await _context.SaveChangesAsync();

            if(trans.Id >0)
                return View("Payment", transaction);
                //return RedirectToAction(nameof(Index));
            //}
            ViewData["BodyType"] = new SelectList(_context.BodyTypes, "Id", "Name", transaction.AppUserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Include(c=> c.Model.Make).Include(c => c.Model).Select(c => new {Id=c.Id,Name=c.Model.Make.Name+" "+ c.Model.Name }), "Id", "Name", transaction.VehicleId);
            return View(transaction);
           
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = _context.Transaction.Where(e => e.Id == id).Include(e=> e.AppUser).Include(e => e.Vehicle).FirstOrDefault();
            if (transaction == null)
            {
                return NotFound();
            }
            Models.DTO.Transaction myTrans = new Models.DTO.Transaction 
            { 
                FirstName = transaction.AppUser.FirstName, 
                LastName = transaction.AppUser.LastName,
                DateOfBirth = transaction.AppUser.DateOfBirth, 
                Email = transaction.AppUser.Email, 
                RegNum = transaction.RegNum, 
                PhoneNumber = transaction.AppUser.PhoneNumber, 
                BodyType = transaction.Vehicle.BodyTypeId, 
                VehicleId = transaction.VehicleId, 
                AppUserId = transaction.AppUserId,
                Id= transaction.Id
            };
            ViewData["BodyType"] = new SelectList(_context.BodyTypes, "Id", "Name", transaction.AppUserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Include(c => c.Model.Make).Include(c => c.Model).Select(c => new { Id = c.Id, Name = c.Model.Make.Name + " " + c.Model.Name }), "Id", "Name", transaction.VehicleId);
            return View(myTrans);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Payment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = _context.Transaction.Where(e => e.Id == id).Include(e => e.AppUser).Include(e => e.Vehicle).FirstOrDefault();
            if (transaction == null)
            {
                return NotFound();
            }
            Models.DTO.Transaction myTrans = new Models.DTO.Transaction
            {
                FirstName = transaction.AppUser.FirstName,
                LastName = transaction.AppUser.LastName,
                DateOfBirth = transaction.AppUser.DateOfBirth,
                Email = transaction.AppUser.Email,
                RegNum = transaction.RegNum,
                PhoneNumber = transaction.AppUser.PhoneNumber,
                BodyType = transaction.Vehicle.BodyTypeId,
                VehicleId = transaction.VehicleId,
                AppUserId = transaction.AppUserId,
                Id = transaction.Id,
                BookingRef = transaction.BookingRef,
                Premium = transaction.Premium
            };
            ViewData["BodyType"] = new SelectList(_context.BodyTypes, "Id", "Name", transaction.AppUserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Include(c => c.Model.Make).Include(c => c.Model).Select(c => new { Id = c.Id, Name = c.Model.Make.Name + " " + c.Model.Name }), "Id", "Name", transaction.VehicleId);
            return View(myTrans);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Models.DTO.Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var myTransaction = _context.Transaction.Find(transaction.Id);

                    Vehicle myVehicle = _context.Vehicles.Where(c => c.Id == transaction.VehicleId).Include(c => c.Model.Make).Include(c => c.BodyType).FirstOrDefault();

                    BodyType myBodyType = _context.BodyTypes.Where(b => b.Id == transaction.BodyType).FirstOrDefault();

                    myTransaction.VehicleMake = myVehicle.Model.Make.Name;
                    myTransaction.VehicleModel = myVehicle.Model.Name;
                    myTransaction.BodyType = myBodyType.Name;
                    myTransaction.Premium = myBodyType.Premium;
                    myTransaction.RegNum = transaction.RegNum;
                    myTransaction.VehicleId = transaction.VehicleId;

                    _context.Update(myTransaction);
                    await _context.SaveChangesAsync();




                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
       
            ViewData["BodyType"] = new SelectList(_context.BodyTypes, "Id", "Name", transaction.AppUserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Include(c => c.Model.Make).Include(c => c.Model).Select(c => new { Id = c.Id, Name = c.Model.Make.Name + " " + c.Model.Name }), "Id", "Name", transaction.VehicleId);

            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.AppUser)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private  string GenerateRandomCode(int l)
        {
            var rng = new Random();
            char[] valid = {
        'A','C','D','E','F','G','H','J','K','L','N','P','Q','R','T','U','V','X','Y','Z','2','3','4','6','7','8','9'};
            StringBuilder sb = new StringBuilder("");
            int i = 0;
            //i < l
            for (i = 0; i <= l; i++)
            {
                sb.Append(valid[rng.Next(valid.Length)]);
            }
            return sb.ToString();

        }
        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
