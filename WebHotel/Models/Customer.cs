using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebHotel.Models
{
    public class Customer
    {
        // Primary key
        [Key, Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 2)]
        [RegularExpression("^[A-Za-z -'][A-Za-z -']*$", ErrorMessage = "Surname must only contian alphabetical letters, hyphen and or an apostrophe.")]
        public string Surname { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z -'][A-Za-z -']*$", ErrorMessage = "Given Name must only contian alphabetical letters, hyphen and or an apostrophe.")]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Given Name")]
        public string GivenName { get; set; }

        [Display(Name = "Post Code")]
        [Required(ErrorMessage = "Postal code required")]
        [RegularExpression(@"^((?!(9))[0-9]{4})$", ErrorMessage = "Invalid Format. Must be 4 digits not starting with 9")]
        public string Postcode { get; set; }

        ICollection<Booking> TheBookings { get; set; }
    }
}
