﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Data;


namespace Meets.Models
{
    public class Helper
    {
        public static bool checkLuhn(string data)
        {
            int sum = 0;
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                int add = (data[i] - '0') * (2 - (i + len) % 2);
                add -= add > 9 ? 9 : 0;
                sum += add;
            }
            return sum % 10 == 0;
        }



        /// <summary>
        /// EventListe liefere Event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Event> LiefereDatabaseEvents(int? id)
        {
            List<Event> sendevent = null;
            using (MeetsEntities con = new MeetsEntities())
            {
                if (id == 0)
                {
                    //Wenn nichts angegeben liefere alle Daten
                    sendevent = con.Events.ToList();
                }
                else
                {
                    sendevent = new List<Event>();
                    foreach (var a in con.Events)
                    {
                        sendevent.Add(a);
                    }
                }
            }
            return sendevent;
        }


        /// <summary>
        /// Methode um einen String konform für MS-SQL Server 2012 'sha2_512' zu hashen
        /// </summary>
        /// <param name="text">der text zum hashen</param>
        /// <returns>Gibt ein ByteArray des gehashten Strings zurück</returns>
        public static Byte[] GetHash(string text)
        {
            //Quelle: Kloiber Christian
            //Damit der Hashwert auch richtig generiert wird MUSS auf Encoding 1252 gestellt werden
            // Erst nachdem das Bytearray mit einem encodierten string nach 1252 erstellt wurde
            // wird dieser mittels ComputeHash gehasht. Danach ist dieser Wert mit
            // MS-SQL Server 2012 VARBINARY(HashByte('sha2_512','HashText')) konform.
            Byte[] hashbytes = null;
            SHA512 alg = SHA512.Create();
            Encoding windows1252 = Encoding.GetEncoding(1252);
            Byte[] result = windows1252.GetBytes(text);
            hashbytes = alg.ComputeHash(result);
            return hashbytes;
        }

        /// <summary>
        /// Email Verständigung zu eingeladenem Event über smtp mit HtmlBody
        /// </summary>
        /// <param name="emailTo">E-Mail Adresse des Empfängers</param>
        /// <param name="id">Id aus Datenbank Meets.Events des einzuladendem Event</param>
        /// <param name="eventTitle">Titel des Events</param>
        /// <returns>string mit Erfolgs Meldung</returns>
        public static string SendEventToEmail(string emailTo, int id, string eventTitle)
        {
            string antwort = null;
            if (emailTo != null)
            {
                try
                {
                    string passwd = "nix";
                    string mailSelf = "Josef.Seiringer@qualifizierung.or.at";

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("noreplay@Meets.at"); //Absender
                    mail.Bcc.Add(emailTo); //Empfänger

                    mail.Subject = "Einladung zu einem Event bei Meets";
                    string linkja = "http://localhost:52111/Events/VerteilerMailAnnahme/" + id;
                    string siteHtml = "<div style='width:400px;height:290px;border:solid 3px #0094ff;padding:10px;margin:20px auto;'><h2>Meets lädt dich zu einem Event ein</h2><p>Um das Event : '" + eventTitle + "'<br/>näher zu betrachten bitte den bestätigungs Link klicken</p><a href='" + linkja + "'>Bestätigen</a></div>";

                    mail.Body = siteHtml;

                    mail.IsBodyHtml = true;

                    //mail.AlternateViews htmlView = AlternateView.CreateAlternateViewFromString()
                    //SmtpClient client = new SmtpClient("srv08.itccn.loc", 25);
                    //SmtpClient client = new SmtpClient("smtp.live.com", 25);
                    SmtpClient client = new SmtpClient("localhost", 25);

                    //!!! nicht SSL im BBRZ verwenden !!!           

                    client.Credentials = new System.Net.NetworkCredential(mailSelf, passwd);

                    //client.EnableSsl = true;

                    client.Send(mail); //Senden 


                    antwort = "Event wurde an Mailadresse versendet";
                    return antwort;
                }
                catch (Exception ex)
                {

                    antwort = ex + "  Problem mit Mailversand";
                }
                
            }

            antwort = "Mailversand nicht erfolgreich";
            return antwort;
        }


        /// <summary>
        /// Methode Mail Senden zu Registrierten Benutzer für Passwortänderung
        /// </summary>
        /// <param name="mailFromUser">Mailadresse von Benutzer</param>
        /// <returns>string Erfolg/Fehlermeldung</returns>

        public static string SendMailRegTo(string mailFromUser)
        {
            string awt = null;
            try
            {
                string passwd = "irgendwas";
                string mailSelf = "bla@qualifizierung.or.at";
                
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("noreplay@Meets.at"); //Absender
                mail.Bcc.Add(mailFromUser); //Empfänger 

                mail.Subject = "Betätigung der Änderung deiner Anmeldedaten bei Meets";
                mail.Body = "Deine Registrierdaten wurden geändert.</br></br><a href='http://localhost:52111/Login/Login' >Bestätigen</a>";
                mail.IsBodyHtml = true;

                //mail.AlternateViews htmlView = AlternateView.CreateAlternateViewFromString()
                //SmtpClient client = new SmtpClient("srv08.itccn.loc", 25);
                //SmtpClient client = new SmtpClient("smtp.live.com", 25);
                SmtpClient client = new SmtpClient("localhost", 25);
                //!!! nicht SSL im BBRZ verwenden !!!           

                client.Credentials = new System.Net.NetworkCredential(mailSelf, passwd);

                client.EnableSsl = true;

                client.Send(mail); //Senden 

                awt = "Email wurde versendet";
                return awt;

            }
            catch (Exception)
            {
                awt = " Fehler beim Versenden der Email";
                return awt;
            }

        }
        /// <summary>
        ///Überladene Methode Mailversand zur User Registrierung
        /// </summary>
        /// <param name="mailFromUser">Mailadresse des empfängers</param>
        /// <param name="id">id aus Datenbank Meets.Member für Übergabe in Datenbank Meets.Membervalidation</param>
        /// <returns>string Erfolg/Fehlermeldung</returns>
        public static string SendMailRegTo(string mailFromUser, int id)
        {
            string awt = null;            
            try
            {
                string passwd = "irgendwas";
                string mailSelf = "bla@qualifizierung.or.at";
                

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("noreplay@Meets.at"); //Absender
                mail.Bcc.Add(mailFromUser); //Empfänger 

                mail.Subject = "Regiestrierbestätigung von Meets";
                string link = "http://localhost:52111/MemberVal/Validate/"+id;
                //string link = "http://localhost:52111/MemberVal/ValidateNew/" + id;
                //mail.Body = "Bitte auf den Registrierungslink klicken um dich bei Meets zu registrieren.</br></br><a href='http://localhost:52111/MemberVal/Validate/id='"+id+"'>Bestätigen</a>";
                mail.Body = "Bitte auf den Registrierungslink klicken um dich bei Meets zu registrieren.</br></br><a href="+link+">Bestätigen</a>";
                mail.IsBodyHtml = true;

                //mail.AlternateViews htmlView = AlternateView.CreateAlternateViewFromString()
                //SmtpClient client = new SmtpClient("srv08.itccn.loc", 25);
                //SmtpClient client = new SmtpClient("smtp.live.com", 25);
                SmtpClient client = new SmtpClient("localhost", 25);
                //!!! nicht SSL im BBRZ verwenden !!!           

                client.Credentials = new System.Net.NetworkCredential(mailSelf, passwd);

                //client.EnableSsl = true;

                client.Send(mail); //Senden 

                awt = "Email wurde versendet";
                return awt;

            }
            catch (Exception)
            {
                awt = "Fehler beim Versenden der Email";
                return awt;
            }

        }


        /// <summary>
        /// Überprüft ob user vorhanden gibt int = 1 zurück wenn nicht int 0 
        /// </summary>
        /// <param name="l"></param>
        /// <returns>INT -1 fehler Datenbankverbindung 1 User vorhanden 0 user nicht vorhanden</returns>
        public static int userLogin(LoginFormModel l)
        {
            int userIs = -1;
            //Übergebenes Passwort hashen und in Var pwhash speichern für Übergabe an DB
            Byte[] pwhash = Helper.GetHash(l.Password);
            using (MeetsEntities cont = new MeetsEntities())
            {                
                try
                {
                    //Von der DB mit den übergebenen Usernamen und PW einen Table mit der UserId/AdminId
                    // anfordern. Wenn kein Eintrag vorhanden ist, ist der User
                    // mit den übergebenen Daten nicht berechtigt

                    Member erg = (from m in cont.Members
                                  where m.email == l.Email &&
                                  m.password == pwhash
                                  select m).FirstOrDefault();
                    if (erg != null)
                    {                                                
                        userIs = 1;
                    }
                    else
                    {
                        userIs = 0;   
                    }                                    
                }
                catch (Exception)
                {
                    //wenn Probleme bei DB-Verbindung
                    userIs = -1;
                }
            }
            return userIs;
        }


    }
}