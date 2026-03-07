using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace serena.Admin
{
    public partial class PaymentMethodsPage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                int id;

                if (TryGetQueryInt("toggle", out id) && id > 0)
                    ToggleUse(id);

                if (TryGetQueryInt("del", out id) && id > 0)
                    DeleteMethod(id);

                if (TryGetQueryInt("edit", out id) && id > 0)
                    LoadForEdit(id);

                BindTable();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var lbl = Find<Label>("lblMsg");
            var hid = Find<HiddenField>("hidId");
            var txt = Find<TextBox>("txtName");
            var chk = Find<CheckBox>("chkUse");

            string name = (txt != null ? txt.Text.Trim() : "");
            bool use = (chk != null ? chk.Checked : false);

            if (string.IsNullOrWhiteSpace(name))
            {
                Show(lbl, "Please enter a name.", false);
                return;
            }

            int id = 0;
            if (hid != null && !string.IsNullOrEmpty(hid.Value)) int.TryParse(hid.Value, out id);

            try
            {
                // Unique name (case-insensitive)
                int exists = Db.Scalar<int>(
                    id > 0
                        ? "SELECT COUNT(*) FROM payment_methods WHERE LOWER(name)=LOWER(@n) AND id<>@id"
                        : "SELECT COUNT(*) FROM payment_methods WHERE LOWER(name)=LOWER(@n)",
                    Db.P("@n", name), Db.P("@id", id));

                if (exists > 0)
                {
                    Show(lbl, "Name already exists.", false);
                    return;
                }

                if (id > 0)
                {
                    Db.Execute(
                        "UPDATE payment_methods SET name=@n, is_use=@u, updated_at=GETDATE() WHERE id=@id",
                        Db.P("@n", name), Db.P("@u", use), Db.P("@id", id));
                    Show(lbl, "Payment method updated.", true);
                }
                else
                {
                    Db.Execute(
                        "INSERT INTO payment_methods (name, is_use, created_at, updated_at) VALUES (@n, @u, GETDATE(), GETDATE())",
                        Db.P("@n", name), Db.P("@u", use));
                    Show(lbl, "Payment method added.", true);
                }

                if (hid != null) hid.Value = "";
                if (txt != null) txt.Text = "";
                if (chk != null) chk.Checked = false;

                BindTable();
            }
            catch
            {
                Show(lbl, "Save failed.", false);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/PaymentMethods.aspx");
        }

        private void LoadForEdit(int id)
        {
            using (var con = Db.Open())
            using (var cmd = new SqlCommand("SELECT id, name, is_use FROM payment_methods WHERE id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (r.Read())
                    {
                        var hid = Find<HiddenField>("hidId");
                        var txt = Find<TextBox>("txtName");
                        var chk = Find<CheckBox>("chkUse");

                        if (hid != null) hid.Value = Convert.ToString(r["id"]);
                        if (txt != null) txt.Text = Convert.ToString(r["name"]);
                        if (chk != null) chk.Checked = Convert.ToBoolean(r["is_use"]);
                    }
                }
            }
        }

        private void DeleteMethod(int id)
        {
            var lbl = Find<Label>("lblMsg");
            try
            {
                int n = Db.Execute("DELETE FROM payment_methods WHERE id=@id", Db.P("@id", id));
                if (n > 0) Show(lbl, "Payment method deleted.", true);
                else Show(lbl, "Not found.", false);
            }
            catch
            {
                Show(lbl, "Delete failed.", false);
            }
        }

        private void ToggleUse(int id)
        {
            try
            {
                Db.Execute("UPDATE payment_methods SET is_use = CASE WHEN is_use=1 THEN 0 ELSE 1 END, updated_at=GETDATE() WHERE id=@id",
                           Db.P("@id", id));
            }
            catch
            {
                // ignore toggle errors for now; message can be shown if you want
            }
        }

        private void BindTable()
        {
            var lit = Find<Literal>("litRows");
            if (lit == null) return;

            var dt = Db.Query("SELECT id, name, is_use FROM payment_methods ORDER BY name ASC;");

            var sb = new StringBuilder();
            if (dt.Rows.Count == 0)
            {
                sb.Append("<tr><td colspan='4' class='text-center text-muted py-3'>No payment methods yet.</td></tr>");
            }
            else
            {
                int i = 0;
                foreach (DataRow r in dt.Rows)
                {
                    i++;
                    int id = Convert.ToInt32(r["id"]);
                    string name = Html(r["name"]);
                    bool use = Convert.ToBoolean(r["is_use"]);

                    sb.Append("<tr>");
                    sb.Append("<td>").Append(i).Append("</td>");
                    sb.Append("<td>").Append(name).Append("</td>");
                    sb.Append("<td>");
                    sb.Append(use
                        ? "<span class='badge text-bg-success'>Active</span>"
                        : "<span class='badge text-bg-secondary'>Inactive</span>");
                    sb.Append("</td>");
                    sb.Append("<td class='text-end'>");
                    sb.Append("<a class='btn btn-sm btn-outline-primary me-2' href='PaymentMethods.aspx?edit=").Append(id).Append("'>Edit</a>");
                    sb.Append("<a class='btn btn-sm btn-outline-warning me-2' href='PaymentMethods.aspx?toggle=").Append(id).Append("'>");
                    sb.Append(use ? "Disable" : "Enable").Append("</a>");
                    sb.Append("<a class='btn btn-sm btn-outline-danger' href='PaymentMethods.aspx?del=").Append(id).Append("' ");
                    sb.Append("onclick=\"return confirm('Delete this payment method?');\">Delete</a>");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
            }
            lit.Text = sb.ToString();
        }

        // ------- helpers (designer-free) -------
        private T Find<T>(string id) where T : Control
        {
            var ph = Master.FindControl("MainContent");
            return ph != null ? FindRecursive<T>(ph, id) : null;
        }

        private static T FindRecursive<T>(Control root, string id) where T : Control
        {
            if (root == null) return null;
            var c = root.FindControl(id) as T;
            if (c != null) return c;
            foreach (Control child in root.Controls)
            {
                var f = FindRecursive<T>(child, id);
                if (f != null) return f;
            }
            return null;
        }

        private static string Html(object o)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(o) ?? "");
        }

        private static bool TryGetQueryInt(string key, out int value)
        {
            value = 0;
            string s = HttpContext.Current.Request.QueryString[key];
            return !string.IsNullOrEmpty(s) && int.TryParse(s, out value);
        }

        private void Show(Label lbl, string msg, bool ok)
        {
            if (lbl == null) return;
            lbl.Text = Server.HtmlEncode(msg);
            lbl.CssClass = ok ? "alert alert-success" : "alert alert-danger";
        }
    }
}
