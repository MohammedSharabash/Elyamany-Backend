using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    [Table("UserChequeViewForTarget")]
    public class UserChequeViewForTarget
    {
        private ElYamanyContext db;
        public UserChequeViewForTarget()
        {
            db = new ElYamanyContext();

        }
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

        [Display(Name = "رقم الشيك")]
        public int ChequeNumber { get; set; }
        [NotMapped]
        [Display(Name = "الحالة")]
        public bool Status { get; set; }
        [NotMapped]
        [Display(Name = "ملاحظة")]
        public string Note { get; set; }
        [NotMapped]
        public long? UserChequeNoteId { get; set; }
        [NotMapped]
        public UserChequeNote UserChequeNote { 
            get
            {
                return db.UserChequeNotes.Where(i => i.UserId == Id && i.ChequeNumber == ChequeNumber).FirstOrDefault();
            }
        }
    }
}