﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBookingSystem.Models
{
    [Table("Booking")]
    public class Booking
    {
        [Key]
        public int booking_id { get; set; }

        public int event_id { get; set; }

        public int user_id { get; set; }

        public DateTime booking_date { get; set; } = DateTime.UtcNow;

        public int number_of_tickets { get; set; }

        // Navigation properties
        [ForeignKey("user_id")]
        public virtual  User User { get; set; }

        [ForeignKey("event_id")]
        public virtual  Event Event { get; set; }
    }
}
