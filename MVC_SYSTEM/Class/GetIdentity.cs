using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCustom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVC_SYSTEM.Class
{
    public class GetIdentity
    {
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        public bool MySuperAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power Admin" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        public bool MyTeamLeader(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Team Leader" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        public bool MyAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Admin", "Admin", "Admin 1", "Viewer" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach(string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true; 
                }
            }

            return result;

        }
        public string MyName(string username)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserShortName.ToString();
            }

            return Name;
        }

        public string MyName2(int? userid)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID == userid).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserShortName.ToString();
            }

            return Name;
        }

        public string MyNameFullName(int? userid)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID == userid).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserFullName.ToString();
            }

            return Name;
        }

        //Aini add 14032023
        public AuthUserLogin AuthUserLogin(string userName)
        {
            var authUserLogin = new AuthUserLogin();
            using (MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth())
            {
                authUserLogin = db.tblUsers.Where(x => x.fldUserName == userName).Join(db.tblRoles, a => a.fldRoleID, b => b.fldRoleID, (a, b) => new AuthUserLogin
                {
                    userID = a.fldUserID,
                    userName = a.fldUserName,
                    Email = a.fldUserEmail,
                    Role = b.fldDescriptionRole
                }).FirstOrDefault();
            }
            return authUserLogin;
        }

        public string Username(int ID)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            string Name;
            Name = "";
            User = db.tblUsers.Where(u => u.fldUserID.Equals(ID)).FirstOrDefault();
            if (User != null)
            {
                Name = User.fldUserName.ToString();
            }

            return Name;
        }
        public int ID(string username)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            int ID = 0;
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            if (User != null)
            {
                ID = User.fldUserID;
            }

            return ID;
        }

        public tblUser GetUserDetail(string username)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            int ID = 0;
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            return User;
        }

        public int ? ClientID(string username)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            tblUser User;
            int ? ID = 0;
            User = db.tblUsers.Where(u => u.fldUserName.Equals(username)).FirstOrDefault();
            if (User != null)
            {
                ID = User.fldClientID;
            }

            return ID;
        }

        public string MyClient(string username)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();

            var clientname = (from a in db.tblUsers
                              join b in db.tblClients on a.fldClientID equals b.fldClientID
                              where a.fldUserName.Equals(username)
                              select b.fldClientName).SingleOrDefault();

            return clientname.ToString();
        }

        public int ? MyCategory(int ? ClientID)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            int? categoryid = 0;

            categoryid = db.tblClients.Where(x => x.fldClientID == ClientID).Select(s => s.fldUserCategory).FirstOrDefault();

            return categoryid;
        }

        public int ? MyCompany(int ? ClientID)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
            int? companyid = 0;

            companyid = db.tblClients.Where(x => x.fldClientID == ClientID).Select(s => s.fldCompanyID).FirstOrDefault();

            return companyid;
        }

        //new identity function
        public bool SuperPowerAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power Admin" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        
        public bool SuperAdmin(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Admin" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool Admin1(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Admin 1", "Viewer" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool Admin2(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Admin 2", "Admin 3" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        public bool SuperPowerUser(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super Power User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }
        public bool SuperUser(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Super User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool NormalUser(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Normal User" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool NegaraSumber(string username)
        {
            string[] roles = new string[] { };
            string[] ourroles = { "Resource" };
            string myrole = "";
            bool result = false;

            using (MVC_SYSTEM_Auth dc = new MVC_SYSTEM_Auth())
            {
                roles = (from a in dc.tblUsers
                         join b in dc.tblRoles on a.fldRoleID equals b.fldRoleID
                         where a.fldUserName.Equals(username)
                         select b.fldRoleName).ToArray<string>();
            }

            myrole = String.Join("", roles); ;

            foreach (string filterrole in ourroles)
            {
                if (myrole == filterrole)
                {
                    result = true;
                }
            }

            return result;
        }

        public int ? getRoleID (int ? id)
        {
            int ? roleid = 0;

            roleid = db.tblUsers.Where(x => x.fldUserID == id).Select(s => s.fldRoleID).SingleOrDefault();

            return roleid;
        }

        public int? getKmplnSyrktID(string username)
        {
            int? KmplnSyrktID = 0;

            KmplnSyrktID = db.tblUsers.Where(u => u.fldUserName.Equals(username)).Select(s=>s.fld_KmplnSyrktID).FirstOrDefault();//db.tblUsers.Where(x => x.fldUserID == id).Select(s => s.fldRoleID).SingleOrDefault();

            return KmplnSyrktID;
        }

        public string getCmpnyShortName(int getuserid, string username)
        {
            GetNSWL GetNSWL = new GetNSWL();
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            string cmpnyshortname = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, username);

            cmpnyshortname = db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_NamaPndkSyarikat).FirstOrDefault();

            return cmpnyshortname;
        }

        public bool getExistingUserIDApp(string UserID, string IC, long? fileid, int? ldgid, int? wlyhid, int? syrktid, int? ngraid)
        {
            bool result = false;

            var getworker = db.tblUserIDApps.Where(x => x.fldUserid == UserID && x.fldNoKP == IC && x.fldLadangID == ldgid && x.fldWilayahID == wlyhid && x.fldSyarikatID == syrktid && x.fldNegaraID == ngraid).Count();

            if (getworker > 1)
            {
                result = true;
            }

            return result;
        }

        public int? RoleID(int? userid)
        {
            int? userRole = db.tblUsers.Where(x => x.fldUserID == userid).Select(s => s.fldRoleID).FirstOrDefault();
            return userRole;
        }

        public List<tblUser> GetListUser()
        {
            return db.tblUsers.ToList();
        }

        public string getWilayahName(string username)
        {
            MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();

            var wilayahname = (from u in db.tblUsers
                               join w in db.tbl_Wilayah on u.fldWilayahID equals w.fld_ID
                               where u.fldUserName.Equals(username)
                               select w.fld_WlyhName).SingleOrDefault();

            return wilayahname;
        }


        //Added by Shazana 31/3/2023
        public int? getRoleIDbyUsername(string username)
        {
            int? roleid = 0;

            roleid = db.tblUsers.Where(x => x.fldUserName == username).Select(s => s.fldRoleID).SingleOrDefault();

            return roleid;
        }

        //Added by Shazana 24/5/2023
        public string getCompanyShortName(int? SyarikatID)
        {

            string cmpnyshortname = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).Select(s => s.fld_NamaPndkSyarikat).FirstOrDefault();
            cmpnyshortname = cmpnyshortname == null ? "" : cmpnyshortname;
            return cmpnyshortname;
        }

        //aini update get latest login access 21062023
        public string AuthUserLogin2(int? userid)
        {
            tblUserAuditTrail AuditTrail;
            string LastAccess;
            LastAccess = "";
            //aini update orderby desc 15062023
            //AuditTrail = db.tblUserAuditTrail.Where(u => u.fld_CreatedBy == userid).Distinct().OrderByDescending(c => c.fld_CreatedDT).FirstOrDefault();
            //if (AuditTrail != null)
            //{
            //    LastAccess = AuditTrail.fld_CreatedDT.ToString();
            //}

            return LastAccess;
        }

        //aini add penjawatan 18072023

        public string Penjawatan(string userName)
        {
            tblUserIDApp Penjawatan;
            string Position;
            Position = "";
            Penjawatan = db.tblUserIDApps.Where(u => u.fldUserid == userName).Distinct().FirstOrDefault();
            if (Penjawatan != null)
            {
                Position = Penjawatan.fldJawatan.ToString();
            }

            return Position;
        }

    }
}