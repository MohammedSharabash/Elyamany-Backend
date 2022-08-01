using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{

    [Table("UserCheque")]
    public class UserCheque
    {
        [Display(Name = "الكود ")]
        public long Id { get; set; }
        [Display(Name = "العضو ")]
        [Required]
        public long UserId { get; set; }
        public User User { get; set; }

        [Display(Name = "تاريخ الإنشاء ")]
        [Required]
        public DateTime CreationDate { get; set; }
        [NotMapped]
        public string CreationDateEgypt
        {
            get
            {
                if (CreationDate != null)
                {
                    var date = CreationDate.AddHours(2);
                    var formatedDate = date.ToString("dd/MM/yyyy HH:mm:ss");

                    return formatedDate;
                }
                return null;
            }
        }

        [Display(Name = "رقم الشيك")]
        [Required]
        public int? ChequeNumber { get; set; }

    }
}