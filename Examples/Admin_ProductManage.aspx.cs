/* 
 * ADMIN PRODUCT MANAGEMENT EXAMPLE
 * Shows validation and business logic for creating/editing products.
 */

using System;
using Saja.BLL;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.Admin
{
    public partial class ProductManage : System.Web.UI.Page
    {
        private ProductBLL productBLL = new ProductBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Check
            if (!SessionHelper.IsAdminLoggedIn())
            {
                Response.Redirect("~/Admin/Login.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Product p = new Product();
            p.Name = txtName.Text;
            p.Description = txtDescription.Text;
            p.Price = decimal.Parse(txtPrice.Text);
            p.Stock = int.Parse(txtStock.Text);
            p.CategoryId = int.Parse(ddlCategory.SelectedValue);
            p.VendorId = int.Parse(txtVendorId.Text); // or from context
            p.IsVisible = chkIsVisible.Checked;
            p.MadeInNepalCertified = chkCertified.Checked;
            p.ImageUrl = "uploads/products/" + fuImage.FileName;

            // Call BLL
            OperationResult result = productBLL.AddProduct(p);

            if (result.Success)
            {
                lblMsg.Text = "Success: " + result.Message;
                // Redirect or clear form
            }
            else
            {
                lblMsg.Text = "Error: " + result.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
