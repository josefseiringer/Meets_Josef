using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class DetailBesaetigungViewModel
    {
        public string Eventdatum { get; set; }
        public string Eventtitel { get; set; }
        public string Bestaetigungszeitpunkt { get; set; }
        public string Eingeladen { get; set; }
        public bool Bestaetigt { get; set; }

    }
}