using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JSONAutocompleteTest.Models
{
    public class MemberViewModel
    {
        #region Eigenschaften/Merkmale
        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Ungültige Emailadresse")]
        [StringLength(30, ErrorMessage = "Maximal 30 Zeichen")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [StringLength(30, ErrorMessage = "Minimal 5 und Maximal 30 Zeichen", MinimumLength = 5)]
        public string Klartextpasswort { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [StringLength(30, ErrorMessage = "Minimal 5 und Maximal 30 Zeichen", MinimumLength = 5)]
        [Compare("Klartextpasswort", ErrorMessage = "Vergleichsfehler")]
        public string RetypePassword { get; set; }

        [Required(ErrorMessage = "Pflichtfeld", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        public System.DateTime Geburtsdatum { get; set; }
        #endregion


        #region Methoden/Verhalten
        /// <summary>
        /// Hashen des Klartextpasswortes zum byte Array für Datenbankspeicherung
        /// </summary>
        /// <param name="klartextpw"></param>
        /// <returns></returns>
        public Byte[] GetHash(string klartextpw)
        {
            //Quelle: Kloiber Christian
            //Damit der Hashwert auch richtig generiert wird MUSS auf Encoding 1252 gestellt werden
            // Erst nachdem das Bytearray mit einem encodierten string nach 1252 erstellt wurde
            // wird dieser mittels ComputeHash gehasht. Danach ist dieser Wert mit
            // MS-SQL Server 2012 VARBINARY(HashByte('sha2_512','HashText')) konform.
            Byte[] hashbytes = null;
            SHA512 alg = SHA512.Create();
            Encoding windows1252 = Encoding.GetEncoding(1252);
            Byte[] result = windows1252.GetBytes(klartextpw);
            hashbytes = alg.ComputeHash(result);
            return hashbytes;
        } 
        #endregion

    }
}