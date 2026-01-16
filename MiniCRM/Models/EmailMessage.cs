using System.ComponentModel.DataAnnotations;

namespace MiniCRM.Models
{
    public class EmailMessage
    {
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string To { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public bool Sent { get; set; }

        // FK do kontaktu
        [Required]
        public int ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}
