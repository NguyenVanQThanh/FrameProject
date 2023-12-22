using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public Boolean Enable { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime? Success { get; set; }
        public string? CustomerId {  get; set; }
        [ForeignKey("CustomerId")]
        public User? User { get; set; }


    }
}
