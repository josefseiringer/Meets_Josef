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
    
    public partial class Eventinvitation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Eventinvitation()
        {
            this.Invitationstatus = new HashSet<Invitationstatu>();
        }
    
        public int id { get; set; }
        public System.DateTime created { get; set; }
        public int event_id { get; set; }
        public string email { get; set; }
    
        public virtual Event Event { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invitationstatu> Invitationstatus { get; set; }
    }
}
