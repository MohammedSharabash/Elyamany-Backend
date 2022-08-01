using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("LevelsSetting")]
    public class LevelsSetting
    {
        [Display(Name = "الكود ")]
        public long Id { get; set; }
        [Display(Name = "اسم المستوى")]
        [Required]
        public string LevelName { get; set; }
        [Display(Name = "اللقب")]
        [Required]
        public string LevelLable { get; set; }
        [Display(Name = "يبدء من ")]
        [Required]
        public int LevelFrom { get; set; }
        [Display(Name = "ينتهي عند ")]
        [Required]
        public int LevelTo { get; set; }
        [Display(Name = "رقم المستوى")]
        [Required]
        public decimal LevelNumebr { get; set; }
        [NotMapped]
        public int? Count { get; set; }
        [NotMapped]
        public long? SumOfPoints { get; set; }
    }
}