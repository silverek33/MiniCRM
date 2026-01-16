using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress, MaxLength(256)]
        public string? Email { get; set; }

        [Phone, MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Company { get; set; }

        // W³aœciciel rekordu (AspNetUsers.Id)
        [Required]
        public string OwnerId { get; set; } = string.Empty;

        // Relacja 1..N do EmailMessage
        public ICollection<EmailMessage> EmailMessages { get; set; } = new List<EmailMessage>();
    }
}