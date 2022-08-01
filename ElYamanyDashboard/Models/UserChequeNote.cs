using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models
{
    [Table("UserChequeNote")]
    public class UserChequeNote
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ChequeId { get; set; }
        public int ChequeNumber { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDate { get; set; }
         public string CreationDateEgypt
        {
            get
            {
                if (CreationDate != null)
                {
                    var date = CreationDate;
                    var formatedDate = date.ToString("dd/MM/yyyy HH:mm:ss");

                    return formatedDate;
                }
                return null;
            }
        }

        public string Note { get; set; }
    }
}