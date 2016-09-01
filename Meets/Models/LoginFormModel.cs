using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class LoginFormModel
    {
        // [] = Attribute - stehen direkt darüber
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        //E-Mail mittels Jquery Validation überprüfen
        [EmailAddress(ErrorMessage = "Ungültige Emailadresse")]
        [StringLength(255, ErrorMessage = "Maximal 255 Zeichen")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required.", AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}