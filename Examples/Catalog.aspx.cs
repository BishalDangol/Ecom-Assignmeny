/* 
 * CATALOG PAGE EXAMPLE
 * Shows how to retrieve and display products with filtering.
 */

using System;
using System.Collections.Generic;
using Saja.BLL;
using Saja.Entities;

public partial class Catalog : System.Web.UI.Page
{
    private ProductBLL productBLL = new ProductBLL();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadProducts();
        }
    }

    private void LoadProducts()
    {
        // Simple call to BLL
        List<Product> products = productBLL.GetAllProducts();
        
        // Bind to Repeater or DataList
        // ProductRepeater.DataSource = products;
        // ProductRepeater.DataBind();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string keyword = txtSearch.Text;
        int? categoryId = null;
        if (ddlCategory.SelectedValue != "0") 
            categoryId = int.Parse(ddlCategory.SelectedValue);

        // Call BLL search with filters
        List<Product> results = productBLL.SearchProducts(keyword, categoryId, null, null);
        
        // ProductRepeater.DataSource = results;
        // ProductRepeater.DataBind();
    }
}
