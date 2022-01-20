using LibraryMs.Models;

namespace LibraryMs.ExcelReports
{
    // Borrowing Report
    public class BorrowingReport
    {

        public DateTime IssueDate { get; set; }
        public string IssuingOfficer { get; set; }
        public DateTime DueDate { get; set; }
        public string StudentName { get; set; }

        public string StudentAdminNo { get; set; }

        public string BookTitle { get; set; }

        public string BookSerialNo { get; set; }

        public string ClassName { get; set; }

    }

    //Book Report
    public class BookReport
    {
        public string BookTitle { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string BookSubject { get; set; }

        public string BookSerialNo { get; set; }

        public string BookClass { get; set; }

        public string RegisteredBy { get; set; }

    }

    // OverDue Report
    public class OverDueReport
    {

        public DateTime IssueDate { get; set; }
        public string IssuingOfficer { get; set; }
        public DateTime DueDate { get; set; }
        public string StudentName { get; set; }

        public string StudentAdminNo { get; set; }

        public string BookTitle { get; set; }

        public string BookSubject { get; set; }

        public string BookSerialNo { get; set; }

        public string BookClass { get; set; }

    }




}

