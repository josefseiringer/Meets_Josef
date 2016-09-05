using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class RegistrierFormModel
    {
        public int IDMember { get; set; }

        public string Klartextpasswort { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        //E-Mail mittels Jquery Validation überprüfen
        [EmailAddress(ErrorMessage = "Ungültige Emailadresse")]
        [StringLength(255, ErrorMessage = "Maximal 255 Zeichen")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string Passwort { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [Compare("Passwort", ErrorMessage = "Vergleichsfehler")]
        public string Passwortvergleich { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        //Variante 3 CustomValidation
        //[CustomValidation(typeof(AlterValidator), "ValidateEndTimeRange")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Geburtsdatum { get; set; }


    }

}