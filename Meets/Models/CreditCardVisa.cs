//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Meets.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CreditCardVisa
    {
        public int id { get; set; }
        public System.DateTime created { get; set; }
        public string memberEmail { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public byte[] visacardnumber { get; set; }
        public byte[] securecode { get; set; }
        public int validmonth { get; set; }
        public int validYear { get; set; }
    }
}
