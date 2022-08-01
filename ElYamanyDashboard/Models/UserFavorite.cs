using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("UserFavorite")]

    public class UserFavorite
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public long SelectedUserId { get; set; }
        [ForeignKey("SelectedUserId")]

        public virtual User  UserSelected{ get; set; }
         public DateTime CreationDate { get; set; }
}
}