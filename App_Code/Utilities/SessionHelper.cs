using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Saja.Entities;

namespace Saja.Utilities
{
    /// <summary>
    /// Utility class for session management.
    /// </summary>
    public static class SessionHelper
    {
        private const string MemberSessionKey = "CurrentUser";
        private const string AdminSessionKey = "CurrentAdmin";
        private const string CartSessionKey = "ShoppingCart";

        /// <summary>
        /// Gets or sets the current logged in member.
        /// </summary>
        public static Member CurrentMember
        {
            get { return HttpContext.Current.Session[MemberSessionKey] as Member; }
            set { HttpContext.Current.Session[MemberSessionKey] = value; }
        }

        /// <summary>
        /// Gets or sets the current logged in admin.
        /// </summary>
        public static Admin CurrentAdmin
        {
            get { return HttpContext.Current.Session[AdminSessionKey] as Admin; }
            set { HttpContext.Current.Session[AdminSessionKey] = value; }
        }

        /// <summary>
        /// Checks if a member is logged in.
        /// </summary>
        public static bool IsMemberLoggedIn()
        {
            return CurrentMember != null;
        }

        /// <summary>
        /// Checks if an admin is logged in.
        /// </summary>
        public static bool IsAdminLoggedIn()
        {
            return CurrentAdmin != null;
        }

        /// <summary>
        /// Clears all sessions (Logout).
        /// </summary>
        public static void Logout()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}
