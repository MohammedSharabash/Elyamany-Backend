namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Area")]
    public partial class Area
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Area()
        {
            Users = new HashSet<User>();
        }

        [Display(Name ="الكود")]
        public long Id { get; set; }

        [Display(Name = "الاسم الانجليزي")]
        public string Name { get; set; }

        [Display(Name = "الاسم العربي")]
        public string NameAR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}
