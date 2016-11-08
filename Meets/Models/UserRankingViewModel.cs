using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class UserRankingViewModel
    {
        //Emailadresse
        public string Email { get; set; }

        //Meine Zusagen
        public int Confirms { get; set; }

        //Meine Gesamtanzahl an Einladungen die ich bekommen habe
        [DisplayName("Gesamteinladungen")]
        public int Eventinvitations { get; set; }
    }
}