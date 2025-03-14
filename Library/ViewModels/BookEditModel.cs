using System.ComponentModel.DataAnnotations;

namespace Library.ViewModels
{
    public class BookEditModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il titolo non può superare i 100 caratteri")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "L'autore è obbligatorio")]
        [StringLength(100, ErrorMessage = "L'autore non può superare i 100 caratteri")]
        public required string Author { get; set; }

        [Required(ErrorMessage = "Il genere è obbligatorio")]
        public int IdGenre { get; set; }

        [Required(ErrorMessage = "Immagine copertina obbligatoria")]
        public IFormFile? File { get; set; }

        public string? Img { get; set; }

        public bool Available { get; set; }
    }
}
