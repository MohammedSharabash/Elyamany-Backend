using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    [Table("ProductView")]
    public class ProductView
    {
        [Display(Name = "الكود")]
        public long Id { get; set; }

        [Display(Name = "الاسم الانجليزي")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "الاسم العربي")]
        [Required]
        public string NameAR { get; set; }

        [Display(Name = "الصورة")]
        public string Image { get; set; }

        [Display(Name = "التصنيف")]
        [Required]
        public long? CategoryId { get; set; }

        [Column(TypeName = "money")]
        [Display(Name = "السعر الحالي")]
        [Required]
        public decimal? CurrentPrice { get; set; }

        [Column(TypeName = "money")]
        [Display(Name = "السعر القديم")]
        [Required]
        public decimal? OldPrice { get; set; }

        [Display(Name = "الكمية المتاحة")]
        [Required]
        public long? AvailableCount { get; set; }

        [Display(Name = "متاح؟")]
        [Required]
        public bool IsAvailable { get; set; }

        public virtual Category Category { get; set; }

        [Display(Name = " العدد المباع حتي الأن")]
        public int SoldProductCount { get; set; }
        [Display(Name = "العدد المباع هذا الشهر")]
        public int SoldProductCountInThisMonth { get; set; }
        [Display(Name = " العدد الكلي هذا الشهر")]
        [NotMapped]
        public int TotalProductCount { get {
                return SoldProductCountInThisMonth + (int)AvailableCount.Value;
            } }

    }
}