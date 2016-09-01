using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meets.Models;

namespace Meets.Controllers
{
    public class MemberValController : Controller
    {
        // GET: MemberVal
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(MembervalidationFormModel mvfm)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                Membervalidation mv = new Membervalidation();
                //kopieren von Db auf Model
                mv.created = DateTime.Now;
                mv.member_id = mvfm.member_id;
                //Entitätsmenge in Datenbank speichern
                con.Membervalidations.Add(mv);
                con.SaveChanges();
            }
            return View();
        }

        
        

        [HttpGet]
        public ActionResult Validate(int id)
        {
            using (MeetsEntities con = new MeetsEntities())
            {
                RegistrierFormModel rfm = new RegistrierFormModel();
                //email anhand der Id ermitteln
                rfm.Email = (from m in con.Members
                             where m.id == id
                             select m.email).FirstOrDefault();
                //id durch Email ermitteln
                rfm.IDMember = (from m in con.Members
                                where m.email == rfm.Email
                                select m.id).FirstOrDefault();

                Membervalidation mfm = new Membervalidation();
                mfm.created = DateTime.Now;
                mfm.member_id = rfm.IDMember;

                int valid = (from m in con.Membervalidations
                             where m.member_id == mfm.member_id
                             select m.id).FirstOrDefault(); 

                if (valid > 0)
                {
                    ViewBag.valErr = "Deine E-Mail ist schon validiert.";
                    return View();
                }
                else
                {
                    con.Membervalidations.Add(mfm);
                    con.SaveChanges();
                    return View();
                   
                }
                
            }            
        }

    }
}