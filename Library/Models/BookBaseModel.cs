using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class BookBaseModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il titolo non può superare i 100 caratteri")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "L'autore è obbligatorio")]
        [StringLength(100, ErrorMessage = "L'autore non può superare i 100 caratteri")]
        public required string Author { get; set; }

        //[Required(ErrorMessage = "Il genere è obbligatorio")]
        //[StringLength(50, ErrorMessage = "Il genere non può superare i 50 caratteri")]
        //public required string Genre { get; set; }
        public int IdGenre { get; set; }

        [ForeignKey(nameof(IdGenre))]
        public GenreBaseModel? Genre { get; set; }

        [Required(ErrorMessage = "Immagine copertina obbligatoria")]
        public required string Img { get; set; }

        public bool Available { get; set; }


    }
}
