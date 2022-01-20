using LibraryMs.Models;

namespace LibraryMs.ExcelReports
{
    public class BorrowingSetUp
    {
        // Borrowings
        public static List<BorrowingReport> BorrowingData(List<Borrowing> books)
        {
            string classname = "classname";
            List<BorrowingReport> report = new();
            foreach (Borrowing b in books)
                {
                    report.Add(new()

                    {
                        IssueDate = b.RegisterDate,
                        IssuingOfficer = b.IssuedBy,
                        DueDate = b.ReturnDate,
                        StudentName = b.CurrentStudent.Name,
                        StudentAdminNo = b.CurrentStudent.AdminNumber,
                        BookTitle = b.CurrentBook.Title,
                        BookSerialNo = b.CurrentBook.SerialNumber,
                        ClassName = classname
                    }
                    );
                };

            return report;

        }

        // Books
        public static List<BookReport> BooksData(List<Book> books)
        {
            List<BookReport> report = new();
            foreach (Book b in books)
            {
                report.Add(new()

                {
                    BookTitle = b.Title,
                    RegisteredDate = b.RegisterDate,
                    BookSubject = b.Subject,
                    BookSerialNo = b.SerialNumber,
                    BookClass = b.Form.Name,
                    RegisteredBy = b.RegisteredBy
                    
                }
                );
            };

            return report;

        }


        // OverDue Issuance
        public static List<OverDueReport> OverDueData(List<Borrowing> books)
        {
            string classname = "classname";
            List<OverDueReport> report = new();
            foreach (Borrowing b in books)
            {
                report.Add(new()

                {
                    IssueDate = b.RegisterDate,
                    IssuingOfficer = b.IssuedBy,
                    DueDate = b.ReturnDate,
                    StudentName = b.CurrentStudent.Name,
                    StudentAdminNo = b.CurrentStudent.AdminNumber,
                    BookTitle = b.CurrentBook.Title,
                    BookSubject = b.CurrentBook.Subject,
                    BookSerialNo = b.CurrentBook.SerialNumber,
                    BookClass = classname
                }
                );
            };

            return report;

        }

    }
}
