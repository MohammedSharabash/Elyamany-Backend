using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("Setting")]
    public class Setting
    {
        [Display(Name ="الكود")]
        public long Id { get; set; }
        [Display(Name = " تشغيل الاوردرات")]
        public bool MakeOrders { get; set; }
        [Display(Name = "رقم الواتساب")]
        public string Whatsapp { get; set; }
    }
}