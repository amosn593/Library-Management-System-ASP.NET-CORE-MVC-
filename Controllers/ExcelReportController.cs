using AspNetCoreHero.ToastNotification.Abstractions;
using LibraryMs.Areas.Identity.Data;
using LibraryMs.Data;
using LibraryMs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LibraryMs.Controllers
{
    public class ExcelReportController : Controller
    {
        private readonly LibraryMsContext _context;

        private readonly INotyfService _notyf;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExcelReportController(
            LibraryMsContext context,
            INotyfService notyf,
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _notyf = notyf;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()
        {
            var file = new FileInfo(@"C:\Users\amos ndonga\Downloads\c#.xlsx");

            var borrowings = from m in _context.Borrowing
                             select m;
            try
            {
                var libraryMsContext = borrowings.Where(b => b.Issued == "Yes")
                    .Include(b => b.CurrentBook)
                    .Include(b => b.CurrentStudent);

                //  _notyf.Success("Success Notification");
                var books = await libraryMsContext.ToListAsync();

                 await Savebook(books, file);
                return RedirectToAction("Index", "Borrowings", new { area = "" });
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task Savebook(List<Borrowing> books, FileInfo file)
        {
            if( file.Exists)
            {
                file.Delete();
            }
            using(var package = new  ExcelPackage(file))
            {
                var wb = package.Workbook.Worksheets.Add("Main Report");

                var range = wb.Cells["A1"].LoadFromCollection(books, true);
                range.AutoFitColumns();

                await package.SaveAsync();

            }
        }
    }
}
