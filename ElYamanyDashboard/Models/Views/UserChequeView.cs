using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    
    public class UserChequeView
    {
        [Display(Name = "كود العضو")]
        public long Id { get; set; }

        [Display(Name = "كود العضو")]
        public string UserCode { get; set; }

        [Display(Name = "الاسم")]
        public string FullName { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Display(Name = "عدد العداد")]
        public int ChequeCount { get; set; }

        [Display(Name = "قيمة الشيك")]
        public int ChequeAmount { get; set; }

 

    }
}