using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Meets.Models
{
    public class CreditcardViewModel
    {
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string Vorname { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string Nachname { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [StringLength(16, ErrorMessage = "Bitte alle 16 Zahlen eingeben", MinimumLength = 16)]
        public string Kartennummer { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public string Sicherheitscode { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public int Kartenmonat { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        public int Kartenjahr { get; set; }

        public string Kartenart { get; set; } //getätigte Auswahl/Antwort der Kombobox
        public List<string> KartenAuswahl { get; set; } //Verfügbaren Werte für die Kombobox

    }
}