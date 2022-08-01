using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("Cheque")]
    public class Cheque
    {
        [Display(Name = "الكود ")]
        public long Id { get; set; }
        [Display(Name = "رقم الشيك ")]
        [Required]
        public int Number { get; set; }
        [Display(Name = "قيمة الشيك ")]
        [Required]
        public int Amount { get; set; }
        [Display(Name = "ملاحظات ")]
        [Required]
        public string Note { get; set; }
        [Display(Name = "اسم الشيك ")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "الرمز ")]
        [Required]
        public string Symbol { get; set; }

        [NotMapped]
        [Display(Name = "عدد المستخدمين")]
        public int UserCount { get; set; }
    }
}