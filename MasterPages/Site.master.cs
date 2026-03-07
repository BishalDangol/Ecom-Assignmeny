using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace serena
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) ApplyActiveNav();
            ShowMemberOrGuest();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SetCartCount();
            ApplyNoCache();
        }

        private void ApplyActiveNav()
        {
            string path = VirtualPathUtility.ToAppRelative(Request.AppRelativeCurrentExecutionFilePath).ToLowerInvariant();
            SetActive("navHome", path.EndsWith("/default.aspx"));
            SetActive("navShop", path.EndsWith("/catalog.aspx"));
            SetActive("navAbout", path.EndsWith("/about.aspx"));
            SetActive("navContact", path.EndsWith("/contact.aspx"));
            SetActive("navFeedback", path.EndsWith("/feedback.aspx"));
        }

        private void SetActive(string anchorId, bool active)
        {
            var a = FindControl(anchorId) as HtmlAnchor;
            if (a == null) return;

            string cls = a.Attributes["class"] ?? "";
            bool has = cls.IndexOf("active", StringComparison.OrdinalIgnoreCase) >= 0;

            if (active && !has)
            {
                a.Attributes["class"] = (cls + " active").Trim();
                a.Attributes["aria-current"] = "page";
            }
            else if (!active && has)
            {
                a.Attributes["class"] = (" " + cls + " ").Replace(" active ", " ").Trim();
                a.Attributes.Remove("aria-current");
            }
        }

        private void ShowMemberOrGuest()
        {
            bool isMember = (Session["MEMBER_ID"] != null);

            var lnkLogin = FindControl("lnkLogin") as HtmlAnchor;
            var lnkRegister = FindControl("lnkRegister") as HtmlAnchor;
            var lnkProfile = FindControl("lnkProfile") as HtmlAnchor;
            var btnLogOut = FindControl("btnLogOut") as LinkButton;

            if (lnkLogin != null) lnkLogin.Visible = !isMember;
            if (lnkRegister != null) lnkRegister.Visible = !isMember;
            if (lnkProfile != null) lnkProfile.Visible = isMember;
            if (btnLogOut != null) btnLogOut.Visible = isMember;
        }

        protected void LogOut_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["MemberToken"] != null)
            {
                Response.Cookies["MemberToken"].Value = string.Empty;
                Response.Cookies["MemberToken"].Expires = DateTime.Now.AddMonths(-20);
            }
            Response.Redirect("~/Default.aspx");
        }

        private void SetCartCount()
        {
            int qty = 0;
            var dict = Session["CART_DICT"] as Dictionary<int, int>;
            if (dict != null)
            {
                foreach (var kv in dict) qty += kv.Value;
            }
            else
            {
                var ht = Session["CART_DICT"] as Hashtable;
                if (ht != null)
                {
                    foreach (DictionaryEntry de in ht)
                        qty += Convert.ToInt32(de.Value);
                }
            }

            Session["CartQty"] = qty;
            var badge = FindControl("cartBadge") as HtmlGenericControl;
            var lit = FindControl("litCartCount") as Literal;

            if (badge != null) badge.Visible = qty > 0;
            if (lit != null) lit.Text = qty.ToString();
        }

        private void ApplyNoCache()
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            Response.Headers["Pragma"] = "no-cache";
        }
    }
}
