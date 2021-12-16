using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryMs.Models
{
    public class Borrowing
    {
        [Key]
        public int Id { get; set; }
        public int StudentID { get; set; }
        public int BookID { get; set; }

        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime RegisterDate { get; set; }
        [Display(Name = "Issued")]
        public string Issued { get; set; } = "Yes";
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime ReturnDate { get; set; }

        [Display(Name = "Returned Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReturnedDate { get; set; } = null;


        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; } = "Yes";

        [Display(Name = "Returned By")]
        public string ReturnedBy { get; set; } = "Yes";

        // Navigation Properties
        [Display(Name = "Student No.")]
        public Student CurrentStudent { get; set; }
        [Display(Name = "Book Serial No.")]
        public Book CurrentBook { get; set; }





    }
}
