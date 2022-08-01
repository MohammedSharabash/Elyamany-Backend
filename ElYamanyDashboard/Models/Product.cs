namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
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
        [NotMapped]
        public string NameWithPrice { get
            {
                return NameAR + "--" + CurrentPrice;
            }
        }
    }
}
