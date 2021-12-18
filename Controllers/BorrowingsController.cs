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

namespace LibraryMs.Controllers
{
    [Authorize]
    public class BorrowingsController : Controller
    {
        private readonly LibraryMsContext _context;

        private readonly INotyfService _notyf;
        public BorrowingsController(LibraryMsContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
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
                    borrowing.Issued = "Yes";
                    borrowing.RegisterDate = DateTime.Now;
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            { 
                var borrowing = await _context.Borrowing.FindAsync(id);
                if (borrowing == null)
                {
                    return NotFound();
                }
                ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
                ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
                return View(borrowing);
            }
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }
        }

        // POST: Borrowings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentID,BookID,RegisterDate ,ReturnDate")] Borrowing borrowing)
        {
            if (id != borrowing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if book already issued
                   // if (_context.Borrowing.Where(c => c.BookID == borrowing.BookID).Where(c => c.Issued == "Yes").Any())
                   // {

                    //}
                    // Checking if RegisterDate(Issued Date) is Greater Than ReturnDate(Due Date)
                    if (borrowing.ReturnDate < borrowing.RegisterDate)
                    {
                        ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
                        ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
                        _notyf.Error("Due Date Must Be Greater Than or Equals To Issue Date.");
                        return View(borrowing);
                        

                    }
                    _context.Update(borrowing);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Book Issuance Updated Successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingExists(borrowing.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
            _notyf.Error("Updated UnSuccessfully, Kindly try Again");
            return View(borrowing);
        }
        */

        // GET: Borrowings/Delete/5
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        

        // POST: Borrowings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowing = await _context.Borrowing.FindAsync(id);
            _context.Borrowing.Remove(borrowing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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
                var borrowing = await _context.Borrowing.FindAsync(id);
                borrowing.Issued = "No";
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
