using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class UserPointsWithMonth
    {
        public int? PersonalPoints { get; set; }
        public int? grouppoint { get; set; }
        public int? ordermonth { get; set; }
        public int? orderYear { get; set; }
        public string UserLevel { get;  set; }
    }
}