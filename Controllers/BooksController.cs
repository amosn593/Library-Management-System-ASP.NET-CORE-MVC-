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
    public class BooksController : Controller
    {
        private readonly LibraryMsContext _context;
        private readonly INotyfService _notyf;
        public BooksController(LibraryMsContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString)
        {
            var books = from m in _context.Book
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    books = books
                    .Where(s => s.SerialNumber!.Contains(searchString));

                    var libraryMsContext = books
                        .OrderBy(c => c.RegisterDate)
                        .Include(b => b.Form);
                    return View(await libraryMsContext.ToListAsync());

                }
                catch (Exception)
                {
                    throw;
                }
                
            }
            try
            {

                var libraryMsContext = books
                    .OrderBy(c => c.RegisterDate)
                    .Include(b => b.Form);
                return View(await libraryMsContext.ToListAsync());

            }
            catch (Exception)
            {
                throw;
            }

        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            { 

                var book = await _context.Book
                    .Include(b => b.Form)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (book == null)
                {
                    return NotFound();
                }

                return View(book);
            }
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            try
            {
                ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name");
                return View();

            }
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }

        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,RegisterDate,Subject,SerialNumber,FormID")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Checking if book already registered
                    if (_context.Book.Where(c => c.SerialNumber == book.SerialNumber).Any())
                    {
                        ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", book.FormID);
                        _notyf.Error("This Book Serial Number is Already Registered.");
                        return View(book);

                    }
                    book.RegisterDate = DateTime.Now;
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Book Registered Successfully.");
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception)
                {
                    throw;
                }

            }
            else
            {
                try
                {
                    ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", book.FormID);
                    return View(book);

                }
                catch (Exception)
                {
                    _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                    return RedirectToAction(nameof(Index));

                }

            }
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var book = await _context.Book.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", book.FormID);
                return View(book);

            }
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }


        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,RegisterDate,Subject,SerialNumber,FormID")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Checking if book already registered
                    if (_context.Book.Where(c => c.SerialNumber == book.SerialNumber).Count()>1)
                    {
                        ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", book.FormID);
                        _notyf.Error("This Book Serial Number is Already Registered.");
                        return View(book);

                    }
                    _context.Update(book);
                    _notyf.Success("Book Updated Successfully.");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            else
            {
                try
                {
                    ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", book.FormID);
                    return View(book);

                }
                catch(Exception)
                {
                    throw;
                }
            }
            
        }

        // GET: Books/Delete/5
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Form)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        */

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            { 
                var book = await _context.Book.FindAsync(id);
                _context.Book.Remove(book);
                await _context.SaveChangesAsync();
                _notyf.Success("Book Deleted Successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch( DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
