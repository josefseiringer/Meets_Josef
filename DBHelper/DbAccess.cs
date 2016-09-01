using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    public class DbAccess
    {
        public static bool addUser(string email, string password, DateTime geburtstag)
        {
            bool success = false;
            //Übergebenes Passwort hashen und in Var pwhash speichern für Übergabe an DB
            Byte[] pwhash = HelperClass.GetHash(password);
            //Übergebenes neues Passwort hashen und in Var pwhash speichern für Übergabe an DB            
            List<int?> userId = new List<int?>();
            //DB-Verbindung hier um User zu speichern
            return success;
        }
     }
}
