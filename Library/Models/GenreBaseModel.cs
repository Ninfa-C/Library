using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class GenreBaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<BookBaseModel> BookCollection { get; set; }
    }
}
