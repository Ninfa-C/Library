using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class LoanBooks
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdLoan { get; set; }

        [Required]
        public Guid IdBook { get; set; }

        [ForeignKey(nameof(IdLoan))]
        public Loan Loan { get; set; }

        [ForeignKey(nameof(IdBook))]
        public BookBaseModel Book { get; set; }

        public bool IsReturned { get; set; } = false;
    }
}
