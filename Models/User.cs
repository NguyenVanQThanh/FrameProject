using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? Address { get; set; }
        [Required]
        public string PostCode { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public string? Phone { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Total must be greater than or equal to 0")]
        public Decimal? Total { get; set; }
        [Required]
        public Boolean Enable {  get; set; }
        public string Role {  get; set; }
        public String printDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

    }
}
