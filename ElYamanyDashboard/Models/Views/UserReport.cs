using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class UserReport
    {

        public long Id { get; set; }

        public long? SponsorId { get; set; }
        [Display(Name ="كود الرعي")]
        public string SponsorUserCode { get; set; }
        [Display(Name = "الإسم")]
        public string FullName { get; set; }
        [Display(Name = "كود العضو")]
        public string UserCode { get; set; }
        [Display(Name = "الهاتف")]
        public string PhoneNumber { get; set; }
        [Display(Name = "النقاط الشخصية")]
        public int PersonalPoints { get; set; }
        [Display(Name = "نقاط المجموعة")]
        public int GroupPoints { get; set; }
        [Display(Name = "المستوى")]
        public string UserLevel { get;  set; }
        [NotMapped]
        [Display(Name = "علامة العضو")]
        public string UserLable { get;  set; }
        [NotMapped]
        public List<UserReport> UserTrees { get;  set; }
        [NotMapped]
        public string AndroidToken { get; set; }
        [NotMapped]
        public string IOSToken { get; set; }
        [NotMapped]
        public long? LevelId { get;  set; }
        public DateTime? CreationDate { get; set; }
    }
}