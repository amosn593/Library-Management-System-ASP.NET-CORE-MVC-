namespace LibraryMs.Models
{
    public class StudentBorrowings
    {
        public Student Student { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}
