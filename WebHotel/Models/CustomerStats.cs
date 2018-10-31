using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class CustomerStats
    {
        public string PostC { get; set; }
        public int PCCount { get; set; }
        public int roomID { get; set; }
        public int roomCount { get; set; }
    }
}
