using System.ComponentModel.DataAnnotations;
namespace LibraryMs.Models
{
    public class Form
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Form Name")]
        [Required]
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
