<%@ Application Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.SessionState" %>

<script runat="server">
    void RegisterRoutes(RouteCollection routes)
    {
        // Pretty route: /product/{slug} → Product.aspx
        routes.MapPageRoute("ProductBySlug", "product/{slug}", "~/Product.aspx");
    }

    void Application_Start(object sender, EventArgs e)
    {
        // Ensure App_Data exists (for logs)
        string appData = Server.MapPath("~/App_Data");
        if (!Directory.Exists(appData)) Directory.CreateDirectory(appData);

        RegisterRoutes(RouteTable.Routes);
    }

    // Lightweight "remember me" rehydrate (Option A). Safe: only runs when Session is available.
    void Application_AcquireRequestState(object sender, EventArgs e)
    {
        if (Context == null) return;

        var h = Context.Handler;
        bool needsSession = (h is IRequiresSessionState) || (h is IReadOnlySessionState);
        if (!needsSession) return;

        if (Session != null && Session["MEMBER_ID"] == null)
        {
            HttpCookie mt = (Request != null) ? Request.Cookies["MemberToken"] : null;
            if (mt == null || string.IsNullOrEmpty(mt.Value)) return;

            try
            {
                DataTable dt = Db.Query(@"
                    SELECT TOP 1 id
                    FROM dbo.members
                    WHERE persistent_token = @t AND token_expires > GETDATE()",
                    Db.P("@t", mt.Value));

                if (dt != null && dt.Rows.Count > 0)
                {
                    int memberId;
                    if (int.TryParse(dt.Rows[0]["id"].ToString(), out memberId) && memberId > 0)
                        Session["MEMBER_ID"] = memberId;
                }
            }
            catch { /* best-effort only */ }
        }
    }

    void Application_End(object sender, EventArgs e) { }

    // Hardened error handler: avoids recursion and always produces a response
    void Application_Error(object sender, EventArgs e)
    {
        Exception ex = Server.GetLastError();
        HttpException httpEx = ex as HttpException;
        int statusCode = (httpEx != null) ? httpEx.GetHttpCode() : 500;

        // Current path (safely)
        string path = "";
        if (Request != null && Request.AppRelativeCurrentExecutionFilePath != null)
            path = Request.AppRelativeCurrentExecutionFilePath;

        // 1) Skip static files to avoid loops (Error.aspx might include CSS/JS/images)
        string ext = System.IO.Path.GetExtension(path);
        if (!string.IsNullOrEmpty(ext))
        {
            ext = ext.ToLowerInvariant();
            if (ext == ".css" || ext == ".js" || ext == ".png" || ext == ".jpg" || ext == ".jpeg" ||
                ext == ".gif" || ext == ".svg" || ext == ".ico" || ext == ".webp" ||
                ext == ".woff" || ext == ".woff2" || ext == ".ttf" || ext == ".eot" || ext == ".axd")
            {
                return; // let static handler/IIS deal with these
            }
        }

        // 2) Don't try to render the error page while already on it
        if (string.Equals(path, "~/Error.aspx", StringComparison.OrdinalIgnoreCase))
            return;

        // 3) Log (best-effort)
        string errorId = Guid.NewGuid().ToString("N");
        try
        {
            string appData = Server.MapPath("~/App_Data");
            if (!Directory.Exists(appData)) Directory.CreateDirectory(appData);

            string logFile = Path.Combine(appData, "errors.log");
            string url = (Request != null && Request.Url != null) ? Request.Url.ToString() : "-";

            using (var sw = new StreamWriter(logFile, true))
            {
                sw.WriteLine("=== {0:u} | {1} | {2} ===", DateTime.UtcNow, errorId, statusCode);
                sw.WriteLine("URL: {0}", url);
                sw.WriteLine("MESSAGE: {0}", ex != null ? ex.Message : "-");
                sw.WriteLine("STACK: {0}", ex != null ? ex.StackTrace : "-");
                sw.WriteLine();
            }
        }
        catch { /* ignore logging failures */ }

        // 4) Redirect to the UI error page; fallback to minimal HTML if redirect fails
        try
        {
            Server.ClearError();
            Response.Redirect("~/Error.aspx?eid=" + errorId, false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch
        {
            try
            {
                Server.ClearError();
                Response.Clear();
                Response.StatusCode = 500;
                Response.TrySkipIisCustomErrors = true;

                string home = "/";
                try { home = VirtualPathUtility.ToAbsolute("~/"); } catch { home = "/"; }

                Response.Write("<!doctype html><html><head><meta charset='utf-8'><title>Error</title></head><body>");
                Response.Write("<h1>Something went wrong</h1>");
                Response.Write("<p>Error ID: " + Server.HtmlEncode(errorId) + "</p>");
                Response.Write("<p><a href='" + Server.HtmlEncode(home) + "'>Back to home</a></p>");
                Response.Write("</body></html>");
                try { Response.End(); } catch { }
            }
            catch { /* swallow */ }
        }
    }

    void Session_Start(object sender, EventArgs e) { }
    void Session_End(object sender, EventArgs e) { }
</script>
