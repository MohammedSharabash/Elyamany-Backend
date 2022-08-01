namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Display(Name = "الكود")]
        public long Id { get; set; }

        [Display(Name = "الاسم الانجليزي")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "الاسم العربي")]
        [Required]
        public string NameAR { get; set; }

        public bool?    Enabled { get; set; }

        [Display(Name = "الصورة")]
        public string Image { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
