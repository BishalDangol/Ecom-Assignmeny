using System;
using System.Text;
using System.Web;
using System.Web.UI;

namespace serena.Site.Account.Orders
{
    public partial class EsewaSuccessPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string dataB64 = Request.QueryString["data"];
            if (string.IsNullOrEmpty(dataB64))
            {
                Response.Redirect("~/Account/Orders/Index.aspx");
                return;
            }

            try
            {
                // Decode data from eSewa v2
                byte[] dataBytes = Convert.FromBase64String(dataB64);
                string decodedJson = Encoding.UTF8.GetString(dataBytes);
                
                // Example JSON: {"transaction_code":"00010OT","status":"COMPLETE","total_amount":"100.0","transaction_uuid":"SS-20240214-12345","product_code":"EPAYTEST","signature":"..."}
                // We should parse it properly. For now, we'll extract the UUID (Order Code).
                
                string orderCode = ExtractValue(decodedJson, "transaction_uuid");
                if (string.IsNullOrEmpty(orderCode)) throw new Exception("Invalid response");

                litOrderCode.Text = orderCode;
                lnkOrder.NavigateUrl = "~/Account/Orders/Detail.aspx?code=" + orderCode;

                // Update order status in DB
                Db.Execute("UPDATE dbo.orders SET status = 'processing' WHERE order_code = @code AND status = 'pending'",
                            Db.P("@code", orderCode));
                
                // Log the payment
                int anyAdminId = 0;
                try { anyAdminId = Db.Scalar<int>("SELECT TOP 1 id FROM dbo.admins ORDER BY id ASC"); } catch { }
                int orderId = Db.Scalar<int>("SELECT id FROM dbo.orders WHERE order_code = @code", Db.P("@code", orderCode));
                
                if (orderId > 0 && anyAdminId > 0)
                {
                    Db.Execute("INSERT INTO dbo.order_logs(order_id, status, admin_id) VALUES (@oid, 'processing', @aid)",
                        Db.P("@oid", orderId), Db.P("@aid", anyAdminId));
                }
            }
            catch
            {
                Response.Redirect("~/Account/Orders/EsewaFailure.aspx");
            }
        }

        private string ExtractValue(string json, string key)
        {
            // Primitive JSON parsing for eSewa response without external dependencies
            string search = "\"" + key + "\":\"";
            int start = json.IndexOf(search);
            if (start == -1) return null;
            start += search.Length;
            int end = json.IndexOf("\"", start);
            if (end == -1) return null;
            return json.Substring(start, end - start);
        }
    }
}
