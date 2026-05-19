using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBookingSystem.Models
{
    [Table("Event")] // Corrected from "Evnet"
    public class Event
    {
        [Key]
        public int event_id { get; set; }

        [Required]
        [StringLength(100)]
        public required string title { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime date { get; set; }

        [Required]
        [StringLength(255)]
        public required string location { get; set; }

        [Required]
        public int total_seats { get; set; }

        [Required]
        public int available_seats { get; set; }

        [StringLength(255)]
        public string? image_url { get; set; }

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
