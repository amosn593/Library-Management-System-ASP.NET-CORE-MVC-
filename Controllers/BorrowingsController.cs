using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryMs.Data;
using LibraryMs.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LibraryMs.Areas.Identity.Data;
using OfficeOpenXml;

namespace LibraryMs.Controllers
{
    [Authorize(Policy = "AllUsers")]
    public class BorrowingsController : Controller
    {
        // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        private readonly LibraryMsContext _context;

        private readonly INotyfService _notyf;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BorrowingsController(
            LibraryMsContext context, 
            INotyfService notyf, 
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _notyf = notyf;
            _userManager = userManager;
           
        }

        // GET: Borrowings
        public async Task<IActionResult> Index(string searchString)
        {

            var borrowings = from m in _context.Borrowing
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                { 
                borrowings = borrowings.Where(b => b.Issued == "Yes")
                    .Include(b => b.CurrentBook)
                    .Where(o => o.CurrentBook.SerialNumber.Contains(searchString))
                    .Include(b => b.CurrentStudent);
                }
                catch(Exception)
                {
                    throw;
                }
            }
            try
            { 
            var libraryMsContext = borrowings.Where(b => b.Issued == "Yes")
                .Include(b => b.CurrentBook)
                .Include(b => b.CurrentStudent);

            //  _notyf.Success("Success Notification");
            return View(await libraryMsContext.ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Borrowings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            { 

            var borrowing = await _context.Borrowing
                .Include(b => b.CurrentBook)
                .Include(b => b.CurrentStudent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowing == null)
            {
                return NotFound();
            }

            return View(borrowing);
            }
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }
        }

        // GET: Borrowings/Create
        public IActionResult Create()
        {
            try
            {
            ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber");
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber");
            return View();
            }
            catch(Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }
        }

        // POST: Borrowings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentID,BookID,RegisterDate, ReturnDate")] Borrowing borrowing)
        {
            // checking if model valid
            if (ModelState.IsValid)
            {
                // check if book already issued
                if(_context.Borrowing.Where(c => c.BookID == borrowing.BookID).Where(c => c.Issued == "Yes").Any())
                {
                    ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
                    ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
                    _notyf.Error("This Book is Already Issued to Another Student.");
                    return View(borrowing);

                }
                //checking if Due date greater than today,now
                if(borrowing.ReturnDate <= DateTime.Now)
                {
                     ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
                     ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
                    _notyf.Error("Due Date Must Be Greater Than or Equals To Todays Date.");
                    return View(borrowing);
                    
                }
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    borrowing.Issued = "Yes";
                    borrowing.RegisterDate = DateTime.Now;
                    borrowing.IssuedBy = user.StaffNumber;
                    _context.Add(borrowing);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Book Issued Successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch( Exception)
                 {
                    _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                    return RedirectToAction(nameof(Index));

                }
            }
            else
            { 
                try
                {
                ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
                ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
                _notyf.Error("Kindly Make sure The Form is Correctly Filled!!!");
                return View(borrowing);
                }
                catch (Exception)
                {
                    _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                    return RedirectToAction(nameof(Index));

                }
            }
        }

        // GET: Borrowings/Edit/5
        /*
       
        */

        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Returned_Book(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var borrowing = await _context.Borrowing.FindAsync(id);
                borrowing.Issued = "No";
                borrowing.ReturnedDate = DateTime.Now;
                borrowing.ReturnedBy = user.StaffNumber;
                await _context.SaveChangesAsync();
                _notyf.Success("Book Returned Successfully.");
                return RedirectToAction(nameof(Index));

            }
            catch(Exception)
            {
                _notyf.Error("Book Returned Unsuccessfully, Kindly try again.");
                return RedirectToAction(nameof(Index));
            }
            
        }
        public async Task<IActionResult> IssuanceHistory(string searchString)
        {
            var borrowings = from m in _context.Borrowing
                             select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    borrowings = borrowings.Where(b => b.Issued == "No")
                        .Include(b => b.CurrentBook)
                        .Where(o => o.CurrentBook.SerialNumber.Contains(searchString))
                        .Include(b => b.CurrentStudent);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            try
            {
                var libraryMsContext = borrowings.Where(b => b.Issued == "No")
                    .Include(b => b.CurrentBook)
                    .Include(b => b.CurrentStudent);
                return View(await libraryMsContext.ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> OverDue(string searchString)
        {
            var borrowings = from m in _context.Borrowing
                             select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    borrowings = borrowings
                        .Where(b => b.Issued == "Yes")
                        .Where(b => b.ReturnDate <= DateTime.Now)
                        .Include(b => b.CurrentBook)
                        .Where(o => o.CurrentBook.SerialNumber.Contains(searchString))
                        .Include(b => b.CurrentStudent);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            try
            {
                var libraryMsContext = borrowings
                    .Where(b => b.Issued == "Yes")
                    .Where(b => b.ReturnDate <= DateTime.Now)
                    .Include(b => b.CurrentBook)
                    .Include(b => b.CurrentStudent);
                return View(await libraryMsContext.ToListAsync());
            }
            catch (Exception)
            {
                throw;
            }
        }


        private bool BorrowingExists(int id)
        {
            return _context.Borrowing.Any(e => e.Id == id);
        }
    }
}
