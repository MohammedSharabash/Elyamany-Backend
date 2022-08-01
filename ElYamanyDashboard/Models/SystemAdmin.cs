namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SystemAdmin")]
    public partial class SystemAdmin
    {
        public long Id { get; set; }
        [Display(Name = "الاسم")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "الهاتف")]
        [Required]
        public string PhoneNumber { get; set; }

        [Display(Name = "البريد الالكتروني")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "الوظيفة")]
        [Required]
        public long? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public string IdentityId { get; set; }

        [NotMapped]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "تاكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "الكلمتان غير متطابقتان من فضل اعد ادخالهما مرة اخرى")]
        public string ConfirmPassword { get; set; }

    }
}
