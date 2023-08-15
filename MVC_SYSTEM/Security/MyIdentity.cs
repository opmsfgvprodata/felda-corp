using MVC_SYSTEM.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MVC_SYSTEM.Security
{
    public class MyIdentity : IIdentity
    {
        public IIdentity Identity { get; set; }
        public tblUser User { get; set; }

        public MyIdentity(tblUser user)
        {
            Identity = new GenericIdentity(user.fldUserName);
            User = user;
        }

        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return Identity.IsAuthenticated; }
        }

        public string Name
        {
            get { return Identity.Name; }
        }

    }
}