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
                catch (Exception)
                {
                    throw;
                }

            }
                try 
            {
                var libraryMsContext = students.Include(s => s.Form);
                return View(await libraryMsContext.ToListAsync());
            }
            catch (Exception)
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

            try
            {
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
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }


        }

        // GET: Students/Create
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
                    // Checking if book already registered
                    if (_context.Student.Where(c => c.AdminNumber == student.AdminNumber).Any())
                    {
                        ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
                        _notyf.Error("This Student Admission Number is Already Registered.");
                        return View(student);

                    }
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Student Registered Successfully.");
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
                    ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
                    return View(student);

                }
                catch (Exception)
                {
                    _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                    return RedirectToAction(nameof(Index));

                }
            }
            
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var student = await _context.Student.FindAsync(id);
                if (student == null)
                {
                    return NotFound();
                }
                ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
                return View(student);

            }
            catch (Exception)
            {
                _notyf.Error("Something Went Wrong, Kindly Try Again!!!");
                return RedirectToAction(nameof(Index));

            }


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
                    // Checking if book already registered
                    if (_context.Student.Where(c => c.AdminNumber == student.AdminNumber).Count() > 1)
                    {
                        ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
                        _notyf.Error("This Student Admission Number is Already Registered.");
                        return View(student);

                    }
                    _context.Update(student);
                    _notyf.Success("Student Updated Successfully.");
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
            else
            {
                try
                {
                    ViewData["FormID"] = new SelectList(_context.Form, "Id", "Name", student.FormID);
                    return View(student);

                }
                catch (Exception)
                {
                    throw;
                }
            }
            
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
                int student_id = borrowing.StudentID;
                await _context.SaveChangesAsync();
                _notyf.Success("Book Returned Successfully.");
                return RedirectToAction(nameof(StudentsController.Details), new { id = student_id });

            }
            catch(Exception)
            {
                _notyf.Error("Book Returned Unsuccessfully, Kindly try again.");
                return RedirectToAction(nameof(Index));
            }
            
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
