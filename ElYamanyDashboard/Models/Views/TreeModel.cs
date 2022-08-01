using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Models.Views
{
    public class TreeModel
    {
        public long id { get; set; }
        public long? pid { get; set; }
        public string personLevel { get; set; }
        public string love { get; set; }
        public List<string> tags { get; set; }
        public int save { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string personalPoints { get; set; }
        public string groupPoints { get; set; }
        public string link { get; set; }
        public string img { get; set; }
    }
}