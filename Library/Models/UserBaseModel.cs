using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class UserBaseModel
    {
        [Key]
        public Guid IdUser { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }
        [Required]
        [StringLength(50)]
        public required string Surname { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
