using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class ChequeView
    {
        
            [Display(Name = "الكود ")]
            public long Id { get; set; }
            [Display(Name = "رقم الشيك ")]
            [Required]
            public int Number { get; set; }
            [Display(Name = "رقيمة الشيك ")]
            [Required]
            public int Amount { get; set; }
            [Display(Name = "ملاحظات ")]
            [Required]
            public string Note { get; set; }
            [Display(Name = "عدد الاعضاء الذين يعدون في الشيك")]
            public int UserCount { get; set; }
        [Display(Name = "عدد من حقق التارجت من الاعضاء")]
        public int? UsersTarget { get; set; }

    }
}