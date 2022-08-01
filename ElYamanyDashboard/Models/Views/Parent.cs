using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class Parent
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public string title { get; set; }
        public string photo { get; set; }
        public List<Parent> children { get; set; }
        public long Id { get;  set; }
        public long SponsorId { get;  set; }
        public string UserLevel { get;  set; }
        public string ChildrenSum { get; set; }
        public bool IsFavorite { get; set; }
    }
}