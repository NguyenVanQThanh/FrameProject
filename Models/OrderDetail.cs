using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class OrderDetail
    {
        public int IdProduct { set; get; }
        public int IdOrder { set; get; }

        [ValidateNever]
        [ForeignKey("IdProduct")]
        public Product Product { set; get; }

        [ValidateNever]
        [ForeignKey("IdOrder")]
        public Order Order { set; get; }

        [Required]
        public decimal Total { set; get; }

        [Required]
        public int Quantity { set; get; }
    }
}