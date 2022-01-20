using AspNetCoreHero.ToastNotification.Abstractions;
using System.IO;
using System.Data;
using LibraryMs.Data;
using LibraryMs.ExcelReports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace LibraryMs.Controllers
{
    public class ExcelReportController : Controller
    {
        private readonly LibraryMsContext _context;

        private readonly INotyfService _notyf;
        

        public ExcelReportController(
            LibraryMsContext context,
            INotyfService notyf
            
            )
        {
            _context = context;
            _notyf = notyf;
            

        }
        // Borrowings report
        public  async Task<IActionResult> BorrowingReport()
        {
            var file = new FileInfo(@"C:\Users\amos ndonga\Downloads\BookIssuance.xlsx");

            var borrowings = from m in _context.Borrowing
                             select m;
            try
            {
                var libraryMsContext = borrowings.Where(b => b.Issued == "Yes")
                    .Include(b => b.CurrentBook)
                    .Include(b => b.CurrentStudent);

                //  _notyf.Success("Success Notification");
                var books = await libraryMsContext.ToListAsync();


                // creating Borrowing Report
                var report =  BorrowingSetUp.BorrowingData(books);

                // Creating a CSV file
                var builder = new StringBuilder();
                builder.AppendLine("IssueDate, IssuingOfficer, DueDate, StudentName, StudentAdminNo.," +
                    "BookTitle, BookSerialNo.");
                foreach (var bk in report)
                {
                    builder.AppendLine($"{bk.IssueDate},{bk.IssuingOfficer},{bk.DueDate}," +
                        $"{bk.StudentName}, {bk.StudentAdminNo}, {bk.BookTitle},{bk.BookSerialNo}");
                }

                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "BookIssuance.csv");


            }

            catch (Exception)
            {
                throw;
            }
        }

       

        //Books Report
        public async Task<IActionResult> BooksReport(string searchString)
        {
            var file = new FileInfo(@"C:\Users\amos ndonga\Downloads\BooksReport.xlsx");

            var books = from m in _context.Book
                        select m;
            try
            {

                var libraryMsContext = books
                    .OrderBy(c => c.RegisterDate)
                    .Include(b => b.Form);

                var report = await libraryMsContext.ToListAsync();

                var data = BorrowingSetUp.BooksData(report);


                await Savebooks(data, file);
                return RedirectToAction("Index", "Books", new { area = "" });

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task Savebooks(List<BookReport> books, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
            using (var package = new ExcelPackage(file))
            {
                var wb = package.Workbook.Worksheets.Add("Main Report");

                var range = wb.Cells["A1"].LoadFromCollection(books, true);
                range.AutoFitColumns();

                await package.SaveAsync();

            }
        }

        // OverDue report
        public async Task<IActionResult> OverDueReport()
        {
            var file = new FileInfo(@"C:\Users\amos ndonga\Downloads\OverDueIssuance.xlsx");

            var borrowings = from m in _context.Borrowing
                             select m;
            try
            {
                var libraryMsContext = borrowings.Where(b => b.Issued == "Yes")
                    .Include(b => b.CurrentBook)
                    .Include(b => b.CurrentStudent);

                //  _notyf.Success("Success Notification");
                var books = await libraryMsContext.ToListAsync();


                // creating Borrowing Report
                var report = BorrowingSetUp.OverDueData(books);


                await SaveOverDue(report, file);
                return RedirectToAction("OverDue", "Borrowings", new { area = "" });


            }

            catch (Exception)
            {
                throw;
            }
        }

        private static async Task SaveOverDue(List<OverDueReport> books, FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
            using (var package = new ExcelPackage(file))
            {
                var wb = package.Workbook.Worksheets.Add("Main Report");

                var range = wb.Cells["A1"].LoadFromCollection(books, true);
                range.AutoFitColumns();

                await package.SaveAsync();

            }
        }


    }

}
