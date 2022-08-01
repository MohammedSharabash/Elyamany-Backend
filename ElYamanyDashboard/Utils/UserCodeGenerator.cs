using ElYamanyDashboard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ElYamanyDashboard.Utils
{
    public class UserCodeGenerator
    {
        private ElYamanyContext db = new ElYamanyContext();

        public string getNewCode(long userCode=0)
        {

           var MAXID=db.Users.AsNoTracking().Max(i => i.Id);
            MAXID = MAXID + 1;
            string otp = "";
            if (userCode>0){otp = userCode.ToString();}else{ otp=MAXID.ToString(); }
           

            int length = otp.Length;
            if (length<7)
            {
                for (int i = 0; i < 7-length; i++)
                {
                    otp = otp + "0";
                }
            }
            string Email = otp + "@ElYamany.com";
            var IsFound = db.Users.AsNoTracking().Where(i => i.UserCode == otp).FirstOrDefault();
            var IsFound2 = db.Database.SqlQuery<string>("select Email from AspNetUsers where Email=N'"+ Email + "'").FirstOrDefault();
            if (IsFound!=null||IsFound2!=null)
            {
                long LongOtp = Convert.ToInt64(otp);
                
               otp= getNewCode(++LongOtp);
            }
            return otp;
        }


        /*
                 public string getNewCode(long userCode=0)
        {

           var MAXID=db.Users.AsNoTracking().Max(i => i.Id);
            MAXID = MAXID + 1;
            //string numbers = "1234567890";
            //string characters = numbers;
            string otp = "";
            if (userCode>0){otp = userCode.ToString();}else{ otp=MAXID.ToString(); }
           

            int length = otp.Length;
            if (length<7)
            {
                for (int i = 0; i < 7-length; i++)
                {
                    otp = otp + "0";
                }
            }
            //for (int i = 0; i < length; i++)
            //{
            //    string character = string.Empty;
            //    do
            //    {
            //        int index = new Random().Next(0, characters.Length);
            //        character = characters.ToCharArray()[index].ToString();
            //    } while (otp.IndexOf(character) != -1);
            //    otp += character;
            //}
            string Email = otp + "@ElYamany.com";
            var IsFound = db.Users.AsNoTracking().Where(i => i.UserCode == otp).FirstOrDefault();
            var IsFound2 = db.Database.SqlQuery<string>("select Email from AspNetUsers where Email=N'"+ Email + "'").FirstOrDefault();
            if (IsFound!=null||IsFound2!=null)
            {
                long LongOtp = Convert.ToInt64(otp);
                
                getNewCode(++LongOtp);
            }
            return otp;
        }

         */
    }
}