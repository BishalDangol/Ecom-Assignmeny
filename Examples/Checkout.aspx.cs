/* 
 * CHECKOUT PAGE EXAMPLE
 * Shows complex order processing and coupon application.
 */

using System;
using System.Collections.Generic;
using Saja.BLL;
using Saja.Entities;
using Saja.Utilities;

public partial class Checkout : System.Web.UI.Page
{
    private OrderBLL orderBLL = new OrderBLL();
    private CouponBLL couponBLL = new CouponBLL();

    protected void btnApplyCoupon_Click(object sender, EventArgs e)
    {
        string code = txtCouponCode.Text;
        decimal subtotal = GetCartSubtotal(); // Local helper to sum current cart
        int memberId = SessionHelper.CurrentMember?.MemberId ?? 0;

        OperationResult result = couponBLL.ValidateAndApply(code, subtotal, memberId);
        
        if (result.Success)
        {
            Coupon coupon = (Coupon)result.Data;
            lblCouponMessage.Text = "Success: " + result.Message;
            lblDiscount.Text = "NPR " + CalculateDiscount(subtotal, coupon);
            // Hide/Show UI elements
        }
        else
        {
            lblCouponMessage.Text = "Error: " + result.Message;
        }
    }

    protected void btnPlaceOrder_Click(object sender, EventArgs e)
    {
        // 1. Check Login
        if (!SessionHelper.IsMemberLoggedIn())
        {
            Response.Redirect("Login.aspx?return=Checkout.aspx");
            return;
        }

        // 2. Prepare Data
        int memberId = SessionHelper.CurrentMember.MemberId;
        List<OrderItem> items = GetCartItems(); // Convert Session cart to List<OrderItem>
        int districtId = int.Parse(ddlDistrict.SelectedValue);
        string address = txtAddress.Text;
        string recipient = txtRecipient.Text;
        string phone = txtPhone.Text;
        int paymentMethodId = int.Parse(rblPayment.SelectedValue);
        string couponCode = txtCouponCode.Text;

        // 3. Call BLL to process entire order
        OperationResult result = orderBLL.CreateOrder(
            memberId, items, districtId, address, recipient, phone, paymentMethodId, couponCode
        );

        if (result.Success)
        {
            // Order created, stock updated, coupon recorded.
            string orderCode = result.Data.ToString(); // Actually we returned ID, could return code
            Response.Redirect("OrderSuccess.aspx?code=" + orderCode);
        }
        else
        {
            lblErrorMessage.Text = result.Message;
        }
    }
}
