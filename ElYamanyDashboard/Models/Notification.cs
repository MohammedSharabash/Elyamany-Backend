namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notification")]
    public partial class Notification
    {
        [Display(Name ="الكود")]
        public long Id { get; set; }

        [Display(Name = "العضو")]
        public long? UserId { get; set; }

        [Display(Name = "نوع الاشعار")]
        [Required]
        public long? NotificationTypeId { get; set; }
        public long? NotificationToId { get; set; }
        [NotMapped]
        public long? LevelSettingId { get; set; }


        [Display(Name = "الرسالة بالانجليزي")]
        [Required]
        public string Message { get; set; }

        [Display(Name = "الرسالة بالعربي")]
        [Required]
        public string MessageAR { get; set; }

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
        public virtual NotificationType NotificationType { get; set; }

        public virtual User User { get; set; }
        [Display(Name = "المستوى")]
        public string UserLevel { get;  set; }
        [Display(Name = "ارسال للكل")]
        [NotMapped]
        public bool ForAll { get; set; }
        [Display(Name = "اسم المنتج انجليزي ")]
        public string ItemName { get; set; }
        [Display(Name = "اسم المنتج عربي")]
        public string ItemNameAR { get; set; }

        public bool IsSeen { get; set; } = false;

    }
}
