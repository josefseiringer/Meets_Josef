using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JSONAutocompleteTest.Models
{
    public class EventViewModel
    {
        public System.DateTime Eventdatum { get; set; }
        public string EvenTitel { get; set; }
        public string Eventbeschreibung { get; set; }
        public bool Viewpublic { get; set; }
        public string Eventstandort { get; set; }
    }
}