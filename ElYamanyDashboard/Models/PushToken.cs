using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("PushToken")]
    public class PushToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string OS { get; set; }
        public long UserId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}