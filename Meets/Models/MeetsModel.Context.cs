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
    
        public virtual DbSet<Configtable> Configtables { get; set; }
        public virtual DbSet<Eventinvitation> Eventinvitations { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Invitationstatu> Invitationstatus { get; set; }
        public virtual DbSet<Memberproperty> Memberproperties { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Membersubscription> Membersubscriptions { get; set; }
        public virtual DbSet<Membervalidation> Membervalidations { get; set; }
        public virtual DbSet<Propertytype> Propertytypes { get; set; }
        public virtual DbSet<BesaetigungDetailView> BesaetigungDetailViews { get; set; }
        public virtual DbSet<View_Event> View_Event { get; set; }
        public virtual DbSet<View_Event_open> View_Event_open { get; set; }
    
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
    
        [DbFunction("MeetsEntities", "fn_detailViewFromUserEmail")]
        public virtual IQueryable<fn_detailViewFromUserEmail_Result> fn_detailViewFromUserEmail(string email)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_detailViewFromUserEmail_Result>("[MeetsEntities].[fn_detailViewFromUserEmail](@email)", emailParameter);
        }
    
        [DbFunction("MeetsEntities", "fn_Show_Event_Table")]
        public virtual IQueryable<fn_Show_Event_Table_Result> fn_Show_Event_Table(string email)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fn_Show_Event_Table_Result>("[MeetsEntities].[fn_Show_Event_Table](@email)", emailParameter);
        }
    
        public virtual int sp_AendereUserdaten(string email, byte[] password)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_AendereUserdaten", emailParameter, passwordParameter);
        }
    
        public virtual int sp_delete_Event(Nullable<int> @event)
        {
            var eventParameter = @event.HasValue ?
                new ObjectParameter("event", @event) :
                new ObjectParameter("event", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_delete_Event", eventParameter);
        }
    
        public virtual int sp_delete_Member(Nullable<int> member)
        {
            var memberParameter = member.HasValue ?
                new ObjectParameter("member", member) :
                new ObjectParameter("member", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_delete_Member", memberParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_GetConfirms(Nullable<int> eventId)
        {
            var eventIdParameter = eventId.HasValue ?
                new ObjectParameter("eventId", eventId) :
                new ObjectParameter("eventId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_GetConfirms", eventIdParameter);
        }
    
        public virtual ObjectResult<sp_holeUserDaten_Result> sp_holeUserDaten(string email)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_holeUserDaten_Result>("sp_holeUserDaten", emailParameter);
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
