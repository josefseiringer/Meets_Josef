//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JSONAutocompleteTest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Memberproperty
    {
        public int id { get; set; }
        public System.DateTime created { get; set; }
        public int member_id { get; set; }
        public int propertytype_id { get; set; }
        public string val { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Propertytype Propertytype { get; set; }
    }
}
