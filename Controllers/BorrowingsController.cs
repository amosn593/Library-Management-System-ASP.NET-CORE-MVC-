using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryMs.Data;
using LibraryMs.Models;

namespace LibraryMs.Controllers
{
    public class BorrowingsController : Controller
    {
        private readonly LibraryMsContext _context;

        public BorrowingsController(LibraryMsContext context)
        {
            _context = context;
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

        // GET: Borrowings/Create
        public IActionResult Create()
        {
            ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber");
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber");
            return View();
        }

        // POST: Borrowings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentID,BookID,RegisterDate, Issued, ReturnDate")] Borrowing borrowing)
        {
            // checking if model valid
            if (ModelState.IsValid)
            {
                //checking if Due date greater than today,now
                if(borrowing.ReturnDate <= DateTime.Now)
                {
                    ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
                    ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
                    return View(borrowing);
                }
                borrowing.Issued = "Yes";
                borrowing.RegisterDate = DateTime.Now;
                _context.Add(borrowing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
            return View(borrowing);
        }

        // GET: Borrowings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowing = await _context.Borrowing.FindAsync(id);
            if (borrowing == null)
            {
                return NotFound();
            }
            ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
            return View(borrowing);
        }

        // POST: Borrowings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentID,BookID,RegisterDate, ReturnDate")] Borrowing borrowing)
        {
            if (id != borrowing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowing);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookID"] = new SelectList(_context.Book, "Id", "SerialNumber", borrowing.BookID);
            ViewData["StudentID"] = new SelectList(_context.Student, "Id", "AdminNumber", borrowing.StudentID);
            return View(borrowing);
        }

        // GET: Borrowings/Delete/5
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
                return RedirectToAction(nameof(Index));

            }
            catch(Exception)
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
