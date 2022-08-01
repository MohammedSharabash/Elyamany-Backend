using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public int ProductCount { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual Product Product { get; set; }
    }
}