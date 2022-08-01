namespace ElYamanyDashboard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [Display(Name ="الكود")]
        public long Id { get; set; }

        [Display(Name = "اسم العضو")]
        [Required]
        public long? UserId { get; set; }

        [Display(Name = "نوع التوصيل")]
        [Required]
        public long? DeliveryTypeId { get; set; }
        [Display(Name = "حالة الطلب")]
       // [Required]
        public long? OrderStatusId { get; set; }

        [Column(TypeName = "money")]
      //  [Required]
        [Display(Name = "اجمالي المنتجات")]
        public decimal? SubTotal { get; set; }

        [Column(TypeName = "money")]
       // [Required]
        [Display(Name = "تكلفة التوصيل")]
        public decimal? DeliveryCost { get; set; }

        [Column(TypeName = "money")]
        //[Required]
        [Display(Name = "اجمالي المبلغ")]
        public decimal? TotalPrice { get; set; }

       // [Required]
        [Display(Name = "النقاط")]
        public int? TotalPoints { get; set; }

      //  [Required]
        [Display(Name = "تاريخ الانشاء")]
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

        [Display(Name = "تاريخ التسليم")]
        [Column(TypeName = "date")]
       // [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveryDate { get; set; }
        [NotMapped]
        public string DeliveryDateEgypt
        {
            get
            {
                if (DeliveryDate != null)
                {
                     var formatedDate = DeliveryDate.Value.ToString("dd/MM/yyyy HH:mm:ss");

                    return formatedDate;
                }
                return null;
            }
        }


        public virtual DeliveryType DeliveryType { get; set; }

        public virtual User User { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public DateTime? CancelationDate { get;  set; }
        [NotMapped]
        public string CancelationDateEgypt
        {
            get
            {
                if (CancelationDate != null)
                {
                    var formatedDate = CancelationDate.Value.ToString("dd/MM/yyyy HH:mm:ss");

                    return formatedDate;
                }
                return null;
            }
        }

        [Display(Name ="سبب الإلغاء")]
        public string CancelationReason { get;  set; }
        [NotMapped]
        public long? ProductId { get; set; }
        [NotMapped]
        public long? ProductCount { get; set; }
    }
}
