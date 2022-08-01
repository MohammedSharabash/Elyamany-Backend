using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("OrderStatus")]
    public class OrderStatus
    {
        [Display(Name = "الكود")]
        public long Id { get; set; }

        [Display(Name = "الاسم الانجليزي")]
        public string Name { get; set; }

        [Display(Name = "الاسم العربي")]
        public string NameAR { get; set; }

    }
}