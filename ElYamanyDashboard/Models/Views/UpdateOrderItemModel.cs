using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class UpdateOrderItemModel
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int ProductCount { get; set; }
        public long? IsError { get; set; }
    }
}