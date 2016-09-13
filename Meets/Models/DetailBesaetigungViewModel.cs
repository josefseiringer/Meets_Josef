using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class DetailBesaetigungViewModel
    {
        public DateTime Eventdatum { get; set; }
        public string Eventtitel { get; set; }
        public DateTime Bestätigungszeitpunkt { get; set; }
        public string Eingeladen { get; set; }
        public bool Bestätigt { get; set; }

    }
}