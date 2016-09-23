using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class InviteToViewModel
    {
        //public int Member_id { get; set; }
        public DateTime EventDatum { get; set; }
        public string Titel { get; set; }
        public string Location { get; set; }
        public string Beschreibung { get; set; }
        public string emailVon { get; set; }
    }
}