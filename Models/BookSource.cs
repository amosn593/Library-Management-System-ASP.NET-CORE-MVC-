using System.ComponentModel.DataAnnotations;

namespace LibraryMs.Models
{
    public class BookSource
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Book Source")]
        [Required]
        public string Book_Source { get; set; }

       // public ICollection<Book>? Book { get; set; }

    }
}
