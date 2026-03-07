using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace serena.Site
{
    public partial class CatalogPage : Page
    {
        protected int? SelectedCategoryId
        {
            get { return ViewState["SelectedCategoryId"] as int?; }
            set { ViewState["SelectedCategoryId"] = value; }
        }

        protected string CurrentSearch
        {
            get { return ViewState["CurrentSearch"] as string ?? string.Empty; }
            set { ViewState["CurrentSearch"] = value ?? string.Empty; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                BindCategories();

                // Read filters from querystring
                int cat;
                if (int.TryParse(Request.QueryString["cat"], out cat))
                    SelectedCategoryId = cat;

                var q = (Request.QueryString["q"] ?? "").Trim();
                CurrentSearch = q;
                txtSearch.Text = CurrentSearch;

                txtPriceMin.Text = Request.QueryString["min"] ?? "";
                txtPriceMax.Text = Request.QueryString["max"] ?? "";

                if (SelectedCategoryId.HasValue)
                {
                    var item = rblCategories.Items.FindByValue(SelectedCategoryId.Value.ToString());
                    if (item != null) item.Selected = true;
                }

                // IMPORTANT: don't reset page here; DataPager reads ?page=
                BindProducts(false);
            }
        }

        protected string GetProductUrl(object nameObj)
        {
            var name = Convert.ToString(nameObj) ?? "";
            var slug = Slugify(name);
            return ResolveUrl("~/product/" + slug);
        }
        private string Slugify(string s)
        {
            // lower-case, keep letters/numbers, turn spaces to '-', collapse repeats
            var sb = new StringBuilder(s.Trim().ToLowerInvariant());
            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) continue;
                if (c == ' ' || c == '_' || c == '-') sb[i] = '-';
                else sb[i] = '-';
            }
            // collapse multiple '-'
            var t = new StringBuilder();
            bool dash = false;
            for (int i = 0; i < sb.Length; i++)
            {
                char c = sb[i];
                if (c == '-')
                {
                    if (!dash) { t.Append('-'); dash = true; }
                }
                else
                {
                    t.Append(c); dash = false;
                }
            }
            // trim leading/trailing '-'
            string res = t.ToString().Trim('-');
            return string.IsNullOrEmpty(res) ? "product" : res;
        }

        private void BindCategories()
        {
            var dt = Db.Query(@"
SELECT id, name
FROM categories
ORDER BY name ASC;");

            rblCategories.Items.Clear();
            rblCategories.Items.Add(new ListItem("All", ""));
            foreach (DataRow r in dt.Rows)
            {
                rblCategories.Items.Add(new ListItem(
                    HttpUtility.HtmlEncode(Convert.ToString(r["name"])),
                    Convert.ToString(r["id"])
                ));
            }
        }

        private void BindProducts(bool resetPageIndex)
        {
            string where = "WHERE p.is_show = 1";
            var parameters = new List<SqlParameter>();

            if (SelectedCategoryId.HasValue)
            {
                where += " AND p.category_id = @cat";
                parameters.Add(new SqlParameter("@cat", SelectedCategoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(CurrentSearch))
            {
                where += " AND (p.name LIKE @q)";
                parameters.Add(new SqlParameter("@q", "%" + CurrentSearch + "%"));
            }

            decimal minPrice, maxPrice;
            if (decimal.TryParse(txtPriceMin.Text, out minPrice))
            {
                where += " AND p.price >= @min";
                parameters.Add(new SqlParameter("@min", minPrice));
            }
            if (decimal.TryParse(txtPriceMax.Text, out maxPrice))
            {
                where += " AND p.price <= @max";
                parameters.Add(new SqlParameter("@max", maxPrice));
            }

            var sql = @"
SELECT p.id, p.name, p.image, p.price, p.stock, c.name AS category_name
FROM products p
LEFT JOIN categories c ON c.id = p.category_id
" + where + @"
ORDER BY p.created_at DESC, p.id DESC;";

            var dt = Db.Query(sql, parameters.ToArray());

            lvProducts.DataSource = dt;

            // With QueryStringField, do NOT force SetPageProperties on (re)binds.
            if (resetPageIndex && pager != null && string.IsNullOrEmpty(pager.QueryStringField))
                pager.SetPageProperties(0, pager.PageSize, false);

            lvProducts.DataBind();
        }

        // Build URL with current filters (cat,q,min,max,page)
        private string BuildUrl(int page)
        {
            var qs = HttpUtility.ParseQueryString(string.Empty);

            if (SelectedCategoryId.HasValue)
                qs["cat"] = SelectedCategoryId.Value.ToString();

            if (!string.IsNullOrWhiteSpace(CurrentSearch))
                qs["q"] = CurrentSearch;

            decimal v;
            if (decimal.TryParse(txtPriceMin.Text, out v)) qs["min"] = v.ToString();
            if (decimal.TryParse(txtPriceMax.Text, out v)) qs["max"] = v.ToString();

            qs["page"] = page.ToString();

            return "Catalog.aspx?" + qs.ToString();
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            int cat;
            SelectedCategoryId = int.TryParse(rblCategories.SelectedValue, out cat) ? (int?)cat : null;
            CurrentSearch = (txtSearch.Text ?? "").Trim();

            // Redirect to page 1 WITH filters in URL
            Response.Redirect(BuildUrl(1), false);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            // Clear and redirect to clean URL (page=1)
            SelectedCategoryId = null;
            CurrentSearch = string.Empty;
            Response.Redirect("Catalog.aspx?page=1", false);
        }

        // For postback paging scenarios (e.g., if QueryStringField removed)
        protected void lvProducts_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            // Keep for safety; not used when QueryStringField is set (links instead of postbacks)
            pager.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindProducts(false);
        }

        protected void lvProducts_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            int productId = Convert.ToInt32(e.CommandArgument);

            TextBox txtQty = (TextBox)e.Item.FindControl("txtQty");
            int qty = 1;
            int.TryParse(txtQty.Text, out qty);

            if (e.CommandName == "Increment")
            {
                txtQty.Text = (qty + 1).ToString();
            }
            else if (e.CommandName == "Decrement")
            {
                if (qty > 1) txtQty.Text = (qty - 1).ToString();
            }
            else if (e.CommandName == "AddToCart")
            {
                AddToCart(productId, qty);
            }
        }

        private void AddToCart(int productId, int qty)
        {
            var cart = GetCart();
            if (cart.ContainsKey(productId))
                cart[productId] += qty;
            else
                cart[productId] = qty;
            SaveCart(cart);
        }

        private Dictionary<int, int> GetCart()
        {
            const string key = "CART_DICT";
            if (Session[key] == null)
                Session[key] = new Dictionary<int, int>();
            return (Dictionary<int, int>)Session[key];
        }

        private void SaveCart(Dictionary<int, int> cart)
        {
            Session["CART_DICT"] = cart;
        }

        protected string GetProductImageUrl(object dbValue)
        {
            var v = dbValue == DBNull.Value ? null : Convert.ToString(dbValue);
            if (!string.IsNullOrWhiteSpace(v))
            {
                string rel = v.TrimStart('~', '/');
                return ResolveUrl("~/" + rel);
            }
            return "https://via.placeholder.com/600x400?text=No+Image";
        }

        protected string Html(object o)
        {
            return HttpUtility.HtmlEncode(Convert.ToString(o) ?? "");
        }
    }
}
