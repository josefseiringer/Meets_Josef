using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class ProfileViewModel
    {

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        //E-Mail mittels Jquery Validation überprüfen
        [EmailAddress(ErrorMessage = "Ungültige Emailadresse")]
        [StringLength(255, ErrorMessage = "Maximal 255 Zeichen")]
        public string Email { get; set; }

        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string NewPasswort { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [Compare("Passwort", ErrorMessage = "Vergleichsfehler")]
        public string Passwortvergleich { get; set; }

        


    }
}