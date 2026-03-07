<%@ Page Language="C#"
    MasterPageFile="~/MasterPages/Site.master"
    AutoEventWireup="true"
    CodeFile="Receipt.aspx.cs"
    Inherits="serena.Site.Account.Orders.ReceiptPage" %>

<asp:Content ID="t" ContentPlaceHolderID="TitleContent" runat="server">
  Receipt
</asp:Content>

<asp:Content ID="m" ContentPlaceHolderID="MainContent" runat="server">
  <div class="container min-vh-60 py-4">

    <!-- Header -->
    <div class="d-flex flex-wrap align-items-center justify-content-between mb-3">
      <h2 class="mb-2 mb-md-0">
        Receipt <span class="text-secondary">(<asp:Literal ID="litOrderCode" runat="server" />)</span>
      </h2>
      <div class="d-flex gap-2">
        <!-- PRINT goes to Download.aspx (new tab) -->
        <asp:HyperLink ID="lnkPrint" runat="server" CssClass="btn btn-dark">
          <i class="fa-solid fa-print me-1"></i> Print
        </asp:HyperLink>
        <asp:HyperLink ID="lnkBackDetail" runat="server" CssClass="btn btn-secondary">
          Back to Order Detail
        </asp:HyperLink>
        <asp:HyperLink ID="lnkBackList" runat="server" CssClass="btn btn-outline-secondary" NavigateUrl="~/Account/Orders/Index.aspx">
          Back to Orders
        </asp:HyperLink>
      </div>
    </div>

    <!-- Alert -->
    <div id="alertMsg" runat="server"></div>

    <!-- Summary + Shipping -->
    <div class="row g-3">
      <div class="col-lg-4">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Summary</h5>
            <dl class="row mb-0">
              <dt class="col-5">Status</dt>
              <dd class="col-7"><span id="litOrderStatus" runat="server" class="badge rounded-pill"></span></dd>

              <dt class="col-5">Payment</dt>
              <dd class="col-7"><asp:Literal ID="litPayment" runat="server" /></dd>

              <dt class="col-5">Order Date</dt>
              <dd class="col-7"><asp:Literal ID="litOrderDate" runat="server" /></dd>

              <dt class="col-5">Total Qty</dt>
              <dd class="col-7"><asp:Literal ID="litTotalQty" runat="server" /></dd>

              <dt class="col-5">Amount</dt>
              <dd class="col-7">RS <asp:Literal ID="litTotalAmt" runat="server" /></dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="col-lg-8">
        <div class="card h-100">
          <div class="card-body">
            <h5 class="card-title">Shipping</h5>
            <div class="row">
              <div class="col-md-6">
                <div class="small text-secondary mb-1">Recipient</div>
                <div><asp:Literal ID="litShipName" runat="server" /></div>
                <div class="text-secondary"><asp:Literal ID="litShipPhone" runat="server" /></div>
              </div>
              <div class="col-md-6">
                <div class="small text-secondary mb-1">Address</div>
                <div><asp:Literal ID="litShipAddr" runat="server" /></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Items -->
    <div class="card mt-4">
      <div class="card-body">
        <h5 class="card-title">Items</h5>
        <asp:Literal ID="litItemsTable" runat="server" />
      </div>
    </div>

  </div>
</asp:Content>
