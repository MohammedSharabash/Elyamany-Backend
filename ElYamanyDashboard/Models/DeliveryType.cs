namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeliveryType")]
    public partial class DeliveryType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliveryType()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public string Name { get; set; }

        public string NameAR { get; set; }
        public decimal? Cost { get; set; }

    }
}
