namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("User")]
    public partial class User
    {
        ElYamanyContext db;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            db = new ElYamanyContext();
         }
        [Display(Name ="كود العضو")]
        public long Id { get; set; }

        [Display(Name = "كود العضو")]
        public string UserCode { get; set; }


        [Display(Name = "الاسم")]
        [Required(ErrorMessage ="من فضلك ادخل الاسم")]
        public string FullName { get; set; }

        [MaxLength(14, ErrorMessage = "يجب الا يزيد الرقم القومي عن 14 رقم")]
        [Display(Name = "الرقم القومي")]
        [Required(ErrorMessage = "من فضلك ادخل الرقم القومي")]
        public string IdentityNumber { get; set; }

        [Display(Name = "العنوان")]
        [Required(ErrorMessage = "من فضلك ادخل العنوان")]
        public string Address { get; set; }

        [Required(ErrorMessage = "من فضلك ادخل علامة مميزة")]
        [Display(Name = "علامة مميزة للعنوان")]
        public string AddressSpecialMark { get; set; }

        [Display(Name = "رقم الدور")]
        [Required(ErrorMessage = "من فضلك ادخل رقم الدور")]
        public string Floor { get; set; }
        [Display(Name = "رقم الشقة")]
        [Required(ErrorMessage = "من فضلك ادخل رقم الشقة")]
        public string ApartmentNumber { get; set; }

        [NotMapped]
        public string FullAddress {
            get {
                return Address + " -" + AddressSpecialMark + " -"+"الدور " + Floor + " -"+"الشقة " + ApartmentNumber;
            } }
        [Display(Name = "الراعي")]
        public long? SponsorId { get; set; }

        [Display(Name = "تاريخ تسجيل العضو")]
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

        [Column(TypeName = "date")]
        [Required]
        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public string DateOfBirthString
        {
            get
            {
                if (DateOfBirth != null)
                {
                    var formatedDate = DateOfBirth.ToString("dd/MM/yyyy HH:mm:ss");

                    return formatedDate;
                }
                return null;
            }
        }

        [MaxLength(11,ErrorMessage ="يجب الا يزيد رقم الهاتف عن 11 رقم")]
        [Display(Name = "رقم الهاتف")]
        [Required(ErrorMessage = "من فضلك ادخل رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public string PhonekeyWithPhoneNumber {
            get
            {
                return "+2" + PhoneNumber;
            }
        }
        [Display(Name = "النوع")]
        [Required]
        public long? GenderId { get; set; }

        [StringLength(50)]
        [Display(Name = "هاتف المنزل")]
        public string HomeNumber { get; set; }

        [Display(Name = "صورة الهوية الامامية")]
        public string IdentityFrontImage { get; set; }
        [Display(Name = "صورة الهوية الخلفية")]
        public string IdentityBackImage { get; set; }

        [Display(Name = "الحساب مفعل")]
        [Required(ErrorMessage = "من فضلك اختر حالة تفعيل حساب العضو")]
        public bool Enabled { get; set; }
        [Display(Name = "صورة العضو ")]
        public string ProfileImage { get; set; }

        [Display(Name = "المنطقة ")]
        [Required(ErrorMessage = "من فضلك اختر منطقة العضو")]
        public long? AreaId { get; set; }


        public int ChequeNumber { get; set; }

        [NotMapped]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "تاكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string IdentityId { get; set; }
        public virtual Area Area { get; set; }
        public virtual Gender Gender { get; set; }
        [NotMapped]
        public string SponsorName
        {
            get
            {
                if (SponsorId==null||SponsorId==0)
                {
                    return "الأدمن";
                }
                var name = db.Users.Where(i => i.Id == SponsorId).Select(i => i.FullName).FirstOrDefault();
                return name;
            }
        }
        [NotMapped]
        public string UserLevel { get;  set; }

        [NotMapped]
        [Display(Name="لقب العضو")]
        public string IsMaster
        {

            get
            {
                var count = db.UserCheques.Where(u => u.UserId == Id).Count();
                if (count >= 8)
                {
                    return "ماستر";
                }
                return "----";
            }
        }

        [NotMapped]
        public int PersonalPoints { get; set; } = 0;
        [NotMapped]
        public int GroupPoints { get; set; } = 0;
    }
}
