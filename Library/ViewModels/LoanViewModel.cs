using System.ComponentModel.DataAnnotations;
using Library.Models;

namespace Library.ViewModels
{
    public class LoanViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "L'email inserita non è valida")]
        public string Email { get; set; }
        public List<Guid> SelectedBooks { get; set; } = new List<Guid>();
    }
}
