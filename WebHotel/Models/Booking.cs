using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Booking
    {
        // Primary Key
        public int ID { get; set; }

        // Foreign Key
        [Range(1, 16)]
        public int RoomID { get; set; }

        // Foreign Key
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [DataType(DataType.Currency)]
        public double Cost { get; set; }

        public Room TheRoom { get; set; }

        public Customer TheCustomer { get; set; }
    }
}
