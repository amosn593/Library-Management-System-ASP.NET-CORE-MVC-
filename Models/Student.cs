using System.ComponentModel.DataAnnotations;

namespace LibraryMs.Models
{

    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Student Name")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Student Admin NO.")]
        [Required]
        public string AdminNumber { get; set; }
        //Navigation
        [Display(Name = "Student Form")]
        public int FormID { get; set; }
        [Display(Name = "Student Form")]
        public Form Form { get; set; }

        public ICollection<Borrowing> Borrowings { get; set; }

    }
}
