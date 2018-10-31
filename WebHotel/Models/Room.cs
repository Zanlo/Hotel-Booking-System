using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Room
    {
        // Primary Key
        public int ID { get; set; }

        [StringLength(1)]
        [RegularExpression("[G1-3]", ErrorMessage = "Select from levels G, 1, 2 or 3.") ]
        public string Level { get; set; }

        [Range(1, 3)]
        public int BedCount { get; set; }

        [Range(50, 300)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        public ICollection<Booking> TheBookings { get; set; }
    }
}
