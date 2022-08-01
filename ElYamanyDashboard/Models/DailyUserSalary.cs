using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("DailyUserSalary")]
    public class DailyUserSalary
    {
        
            [Display(Name = "الكود ")]
            public long Id { get; set; }
            [Display(Name = "العضو ")]
            [Required]
            public long UserId { get; set; }

            public virtual User User { get; set; }
            [Display(Name = "النقاط الشخصية ")]
            [Required]
            public int PersonalPoints { get; set; }
            [Display(Name = "نقاط المجموعة ")]
            [Required]
            public int GroupPoints { get; set; }
            [Display(Name = "الخصم ")]
            [Required]
            public decimal Deduction { get; set; }
            [Display(Name = "قيمة الراتب ")]
            [Required]
            public decimal SalaryAmount { get; set; }

            [Display(Name = "قيمة الراتب ")]
            [NotMapped]
            public int SalaryAmountInt
            {
                get
                {
                    if (SalaryAmount > 0)
                    {
                        var amount = Convert.ToInt32(SalaryAmount);
                        return amount;
                    }
                    return 0;
                }
            }
            [Display(Name = "تاريخ الاصدار ")]
            [Required]
            public DateTime? CreationDate { get; set; }

            [NotMapped]
            public string CreationDateEgypt
            {
                get
                {
                    if (CreationDate != null)
                    {
                        var date = CreationDate.Value.AddHours(2);
                        var formatedDate = date.ToString("dd/MM/yyyy HH:mm:ss");

                        return formatedDate;
                    }
                    return null;
                }
            }

            [Display(Name = "تم السداد ")]
            [Required]
            public bool Status { get; set; }

            public DateTime? PayDate { get; set; }
            [NotMapped]
            public string PayDateEgypt
            {
                get
                {
                    if (PayDate != null)
                    {
                        var date = PayDate.Value.AddHours(2);
                        var formatedDate = date.ToString("dd/MM/yyyy HH:mm:ss");

                        return formatedDate;
                    }
                    return null;
                }
            }
        
    }

}