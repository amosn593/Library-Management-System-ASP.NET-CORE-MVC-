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

namespace LibraryMs.Controllers
{
    public class StudentsController : Controller
    {
        private readonly LibraryMsContext _context;
        private readonly INotyfService _notyf;
        public StudentsController(LibraryMsContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Students
        public async Task<IActionResult> Index(string searchString)
        {
            var students = from m in _context.Student
                             select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    var libraryMsContext = students
                        .Where(c => c.AdminNumber.Contains(searchString))
                        .Include(s => s.Form);
                    return View(await libraryMsContext.ToListAsync());
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

            }
                try 
            {
                var libraryMsContext = students.Include(s => s.Form);
                return View(await libraryMsContext.ToListAsync());
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = new StudentBorrowings();

            model.Student = await _context.Student
                .Include(s => s.Form)
                .FirstOrDefaultAsync(m => m.Id == id);

            model.Borrowings = await _context.Borrowing
                .Where(d => d.StudentID == id)
                .Where(c => c.Issued == "Yes")
                .Include(b => b.CurrentBook)
                .Include(b => b.CurrentStudent)
                .ToListAsync();

            if (model.Student == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AdminNumber,FormID")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                { 
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AdminNumber,FormID")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
            return View(student);
        }

        // GET: Students/Delete/5
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Form)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        */

        // POST: Students/Delete/5
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
                var student = await _context.Student.FindAsync(id);
                _context.Student.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
