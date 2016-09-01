﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MeetsEntities : DbContext
    {
        public MeetsEntities()
            : base("name=MeetsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<View_Event> View_Event { get; set; }
        public virtual DbSet<View_Event_open> View_Event_open { get; set; }
        public virtual DbSet<Membervalidation> Membervalidations { get; set; }
    
        [DbFunction("MeetsEntities", "fn_check_user_Table")]
        public virtual IQueryable<fn_check_user_Table_Result> fn_check_user_Table(string email, byte[] password)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_check_user_Table_Result>("[MeetsEntities].[fn_check_user_Table](@email, @password)", emailParameter, passwordParameter);
        }
    
        public virtual int sp_RegisterUser(string email, byte[] password, Nullable<System.DateTime> dateOfBirth)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(byte[]));
    
            var dateOfBirthParameter = dateOfBirth.HasValue ?
                new ObjectParameter("dateOfBirth", dateOfBirth) :
                new ObjectParameter("dateOfBirth", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_RegisterUser", emailParameter, passwordParameter, dateOfBirthParameter);
        }
    }
}
