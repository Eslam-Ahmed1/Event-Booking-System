using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBookingSystem.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        [StringLength(100)]
        public required string name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public required string email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [StringLength(255)]
        public required string password { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [DefaultValue("user")]
        [StringLength(100)]
        public string role { get; set; } = "user";

        [Required]
        [Phone]
        [StringLength(100)]
        public required string phone { get; set; }

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}