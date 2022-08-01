namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("ContactUs")]
    public partial class ContactUs
    {
        [Display(Name ="الكود")]
        public long Id { get; set; }

        [Display(Name = "العضو")]
        public long? UserId { get; set; }

        [Display(Name = "الشكوى")]
        public string Message { get; set; }

        [Display(Name = "تاريخ ارسالها")]
        public DateTime? CreationDate { get; set; }
        [Display(Name = "Creation Date")]
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

        [Display(Name = "تم الإطلاع")]
        public bool IsReviewed { get; set; }

        public virtual User User { get; set; }
    }
}
