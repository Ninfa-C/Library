using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class Loan
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid IdUser { get; set; }

        [ForeignKey(nameof(IdUser))]
        public UserBaseModel? User { get; set; }

        [Required]
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;

        public ICollection<LoanBooks> LoanBooks { get; set; }

    }
}
