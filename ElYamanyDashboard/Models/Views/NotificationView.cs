using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class NotificationView
    {
        [Display(Name = "الرسالة بالانجليزي")]
        [Required]
        public string Message { get; set; }

        [Display(Name = "الرسالة بالعربي")]
        [Required]
        public string MessageAR { get; set; }

        public DateTime? CreationDate { get; set; }
        [Display(Name = "المستوى")]
        public string UserLevel { get; set; }
        [NotMapped]
        public string CreationDatestring { get {

                if (CreationDate != null)
                {
                    var date = CreationDate.Value.AddHours(2);
                    var formatedDate = date.ToString("dd/MM/yyyy HH:mm:ss");

                    return formatedDate;
                }
                return null;
            } }

    }
}