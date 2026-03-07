using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace serena.Admin
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Already signed in? Go straight to the dashboard.
            if (Context != null && Context.User != null && Context.User.Identity.IsAuthenticated)
            {
                SafeRedirect("~/Admin/Dashboard.aspx"); return;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = GetText("txtUser");
            string password = GetText("txtPass");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter username and password.");
                return;
            }

            try
            {
                int adminId = 0;
                string dbHash = null;

                using (var con = Db.Open())
                using (var cmd = new SqlCommand("SELECT id, [password] FROM dbo.admins WHERE username=@u", con))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (r.Read())
                        {
                            adminId = r.IsDBNull(0) ? 0 : r.GetInt32(0);
                            dbHash = r.IsDBNull(1) ? null : r.GetString(1);
                        }
                    }
                }

                if (adminId <= 0 || dbHash == null || !dbHash.Equals(Sha256Hex(password), StringComparison.OrdinalIgnoreCase))
                {
                    ShowError("Invalid username or password.");
                    return;
                }

                // 1) Normal auth cookie (persistent=true)
                FormsAuthentication.SetAuthCookie(username, true);

                // 2) Persistent token for silent re-login after app recycles
                string token = Guid.NewGuid().ToString("N");
                DateTime expires = DateTime.UtcNow.AddDays(14);

                try
                {
                    using (var con2 = Db.Open())
                    using (var cmd2 = new SqlCommand(
                        "UPDATE dbo.admins SET persistent_token=@t, token_expires=@e WHERE id=@id", con2))
                    {
                        cmd2.Parameters.AddWithValue("@t", token);
                        cmd2.Parameters.AddWithValue("@e", expires);
                        cmd2.Parameters.AddWithValue("@id", adminId);
                        cmd2.ExecuteNonQuery();
                    }
                }
                catch
                {
                    // If columns missing, skip silently; normal auth still works.
                }

                var tk = new HttpCookie("AdminToken", token);
                tk.HttpOnly = true;
                // If you move to HTTPS: tk.Secure = true;
                tk.Expires = expires;
                Response.Cookies.Add(tk);

                // Dashboard
                SafeRedirect("~/Admin/Dashboard.aspx");
            }
            catch
            {
                ShowError("Login failed. Please try again.");
            }
        }

        // ---------- helpers ----------
        private string GetText(string id)
        {
            var c = FindControl(id) as System.Web.UI.WebControls.TextBox;
            return c != null ? c.Text.Trim() : string.Empty;
        }

        private void ShowError(string message)
        {
            var lbl = FindControl("lblMsg") as System.Web.UI.WebControls.Label;
            if (lbl != null) lbl.Text = Server.HtmlEncode(message);
        }

        private static string Sha256Hex(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
                var sb = new StringBuilder(bytes.Length * 2);
                for (int i = 0; i < bytes.Length; i++) sb.Append(bytes[i].ToString("X2"));
                return sb.ToString();
            }
        }

        private void SafeRedirect(string url)
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
