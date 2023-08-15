using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class SendEmailNotification
    {
        errorlog geterror = new errorlog();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();

        //new Class
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        public bool SendEmail(string subject, string msg, string[] to, string[] cc, string[] bcc)
        {
            string[] multirecpt = new string[] { };
            bool result = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(getemailsmtp());

                mail.From = new MailAddress(getemailfrom(), getemailfromname());
                if (to != null)
                {
                    foreach (var recpt in to)
                    {
                        mail.To.Add(recpt);
                    }
                }
                else
                {
                    mail.To.Add(getemailto());
                }

                if (cc != null)
                {
                    foreach (var recpt in cc)
                    {
                        mail.CC.Add(recpt);
                    }
                }
                else
                {
                    mail.CC.Add(getemailcc());
                }

                if (bcc != null)
                {
                    foreach (var recpt in bcc)
                    {
                        mail.Bcc.Add(recpt);
                    }
                }

                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                SmtpServer.Port = getemailport();
                var username = getemailfrom();
                var password = getemailpass();
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                //SmtpServer.EnableSsl = true;
                //added by faeza on 19.06.2023
                SmtpServer.EnableSsl = getssl();
                SmtpServer.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                result = false;
            }

            return result;
        }
        //added by kamalia 24/11/21
        private bool getssl()
        {
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            bool getresult = bool.Parse(parser.GetSetting("configdeclaration", "emailssl"));

            return getresult;
        }
        public bool SendEmail2(string subject, string msg, string[] to, string[] cc, string[] bcc)
        {
            string[] multirecpt = new string[] { };
            bool result = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(getemailsmtp());

                mail.From = new MailAddress(getemailfrom(), getemailfromname());
                if (to != null)
                {
                    foreach (var recpt in to)
                    {
                        mail.To.Add(recpt);
                    }
                }
                else
                {
                    mail.To.Add(getemailto());
                }

                if (cc != null)
                {
                    foreach (var recpt in cc)
                    {
                        mail.CC.Add(recpt);
                    }
                }
                else
                {
                    mail.CC.Add(getemailcc());
                }

                if (bcc != null)
                {
                    foreach (var recpt in bcc)
                    {
                        mail.Bcc.Add(recpt);
                    }
                }

                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                SmtpServer.Port = getemailport();
                var username = getemailfrom();
                var password = getemailpass();
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                //added by faeza on 22.02.2021
                SmtpServer.EnableSsl = getssl();
                //SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                result = false;
            }

            return result;
        }
        //end
        public bool SendEmailToHQ(string subject, string msg, string to, string[] cc, string[] bcc)
        {
            string[] multirecpt = new string[] { };
            bool result = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(getemailsmtp());

                mail.From = new MailAddress(getemailfrom(), getemailfromname());
                if (to != null || to != "")
                {
                    mail.To.Add(to);
                }
                else
                {
                    mail.To.Add(getemailto());
                }

                if (cc != null)
                {
                    foreach (var recpt in cc)
                    {
                        mail.CC.Add(recpt);
                    }
                }
                else
                {
                    mail.CC.Add(getemailcc());
                }

                if (bcc != null)
                {
                    foreach (var recpt in bcc)
                    {
                        mail.Bcc.Add(recpt);
                    }
                }
                
                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                SmtpServer.Port = getemailport();
                var username = getemailfrom();
                var password = getemailpass();
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                //SmtpServer.EnableSsl = true;
                //added by faeza on 19.06.2023
                SmtpServer.EnableSsl = getssl();
                SmtpServer.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                result = false;
            }

            return result;
        }

        public bool SendEmailToWilayah(string subject, string msg, string to, string[] cc, string[] bcc)
        {
            string[] multirecpt = new string[] { };
            bool result = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(getemailsmtp());

                mail.From = new MailAddress(getemailfrom(), getemailfromname());
                if (to != null || to != "")
                {
                    mail.To.Add(to);
                }
                else
                {
                    mail.To.Add(getemailto());
                }

                if (cc != null)
                {
                    foreach (var recpt in cc)
                    {
                        mail.CC.Add(recpt);
                    }
                }
                else
                {
                    mail.CC.Add(getemailcc());
                }

                if (bcc != null)
                {
                    foreach (var recpt in bcc)
                    {
                        mail.Bcc.Add(recpt);
                    }
                }

                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                SmtpServer.Port = getemailport();
                var username = getemailfrom();
                var password = getemailpass();
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                //SmtpServer.EnableSsl = true;
                //added by faeza on 19.06.2023
                SmtpServer.EnableSsl = getssl();
                SmtpServer.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                result = false;
            }

            return result;
        }

        public bool SendEmailToLadang(string subject, string msg, string to, string[] cc, string[] bcc)
        {
            string[] multirecpt = new string[] { };
            bool result = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(getemailsmtp());

                mail.From = new MailAddress(getemailfrom(), getemailfromname());
                if (to != null || to != "")
                {
                    mail.To.Add(to);
                }
                else
                {
                    mail.To.Add(getemailto());
                }

                if (cc != null)
                {
                    foreach (var recpt in cc)
                    {
                        mail.CC.Add(recpt);
                    }
                }
                else
                {
                    mail.CC.Add(getemailcc());
                }

                if (bcc != null)
                {
                    foreach (var recpt in bcc)
                    {
                        mail.Bcc.Add(recpt);
                    }
                }

                mail.Subject = subject;
                mail.Body = msg;
                mail.IsBodyHtml = true;

                SmtpServer.Port = getemailport();
                var username = getemailfrom();
                var password = getemailpass();
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                //SmtpServer.EnableSsl = true;
                //added by faeza on 19.06.2023
                SmtpServer.EnableSsl = getssl();
                SmtpServer.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                result = false;
            }

            return result;
        }

        public bool CheckEmailNotiStatus(int NegaraID, int? SyarikatID, int? WilayahID, int LadangID, string flag, string source)
        {
            bool status = false;

            var emailstatus = dbC.tblEmailNotiStatus.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldEmailNotiFlag == flag && x.fldEmailNotiSource == source && x.fldEmailNotiStatus == 1).FirstOrDefault();

            if (emailstatus == null)
            {
                status = true;
            }

            return status;
        }

        public void InsertIntotblEmailNotiStatus(int NegaraID, int? SyarikatID, int? WilayahID, int LadangID, string flag, string Desc, string source, int status)
        {
            var getEmailNotiStatus = dbC.tblEmailNotiStatus.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldEmailNotiFlag == flag && x.fldEmailNotiSource == source).FirstOrDefault();

            if (getEmailNotiStatus == null)
            {
                ModelsCorporate.tblEmailNotiStatu tblEmailNotiStatu = new ModelsCorporate.tblEmailNotiStatu();

                tblEmailNotiStatu.fldEmailNotiDesc = Desc;
                tblEmailNotiStatu.fldEmailNotiFlag = flag;
                tblEmailNotiStatu.fldEmailNotiStatus = status;
                tblEmailNotiStatu.fldNegaraID = NegaraID;
                tblEmailNotiStatu.fldSyarikatID = SyarikatID;
                tblEmailNotiStatu.fldWilayahID = WilayahID;
                tblEmailNotiStatu.fldLadangID = LadangID;
                tblEmailNotiStatu.fldEmailNotiSource = source;
                tblEmailNotiStatu.fldDateTimeStamp = timezone.gettimezone();

                dbC.tblEmailNotiStatus.Add(tblEmailNotiStatu);
            }
            else
            {
                getEmailNotiStatus.fldEmailNotiStatus = status;
                dbC.Entry(getEmailNotiStatus).State = EntityState.Modified;
            }
            dbC.SaveChanges();
        }

        private int getemailport()
        {
            int getresult = 0;
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = int.Parse(parser.GetSetting("configdeclaration", "emailport"));

            return getresult;
        }

        private string getemailto()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "emailto");

            return getresult;
        }

        private string getemailcc()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "emailcc");

            return getresult;
        }

        private string getemailfrom()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "emailfrom");

            return getresult;
        }

        private string getemailfromname()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "emailfromname");

            return getresult;
        }

        private string getemailpass()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "emailpass");

            return getresult;
        }

        private string getemailsmtp()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "emailsmtp");

            return getresult;
        }
    }
}