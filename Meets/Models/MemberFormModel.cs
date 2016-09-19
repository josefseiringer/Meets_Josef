using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meets.Models
{
    public class MemberFormModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public System.DateTime created { get; set; }
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Ungültige Emailadresse")]
        [StringLength(255, ErrorMessage = "Maximal 255 Zeichen")]
        public string email { get; set; }
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public byte[] password { get; set; }
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime dateofbirth { get; set; }
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string Klartextpasswort { get; set; }
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string RetypePasswort { get; set; }

    }
}