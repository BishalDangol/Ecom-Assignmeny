<%@ Page Language="C#" MasterPageFile="~/MasterPages/Admin.master"
    AutoEventWireup="true" CodeFile="Dashboard.aspx.cs"
    Inherits="serena.Admin.Dashboard" %>

<asp:Content ID="c1" ContentPlaceHolderID="TitleContent" runat="server">Overview | Saja Admin</asp:Content>

<asp:Content ID="c2" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Page Header -->
    <div class="mb-12">
        <h2 class="text-3xl font-serif mb-2">Workspace Overview</h2>
        <p class="text-xs uppercase tracking-widest text-gray-400 font-bold">Real-time performance and insights</p>
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8 mb-16">
        <!-- Card 1 -->
        <div class="bg-white border-l-4 border-primary p-8 shadow-sm hover:shadow-xl transition-all duration-500">
            <div class="flex justify-between items-start mb-6">
                <div class="w-10 h-10 bg-off-white flex items-center justify-center text-primary rounded-sm">
                    <i class="fa-solid fa-shopping-bag text-sm"></i>
                </div>
                <a class="text-[10px] uppercase tracking-widest font-bold text-primary hover:underline" href="<%: ResolveUrl("~/Admin/Orders.aspx?filter=today") %>">Details</a>
            </div>
            <div class="text-[10px] uppercase tracking-widest font-bold text-gray-400 mb-2">Orders Today</div>
            <div class="text-4xl font-serif text-text-dark"><asp:Literal ID="litOrdersToday" runat="server" /></div>
        </div>

        <!-- Card 2 -->
        <div class="bg-white border-l-4 border-accent p-8 shadow-sm hover:shadow-xl transition-all duration-500">
            <div class="flex justify-between items-start mb-6">
                <div class="w-10 h-10 bg-off-white flex items-center justify-center text-accent rounded-sm">
                    <i class="fa-solid fa-dollar-sign text-sm"></i>
                </div>
                <span class="text-[10px] uppercase tracking-widest font-bold text-gray-400">Excl. Cancelled</span>
            </div>
            <div class="text-[10px] uppercase tracking-widest font-bold text-gray-400 mb-2">Revenue Today</div>
            <div class="text-4xl font-serif text-text-dark"><asp:Literal ID="litRevenueToday" runat="server" /></div>
        </div>

        <!-- Card 3 -->
        <div class="bg-white border-l-4 border-orange-200 p-8 shadow-sm hover:shadow-xl transition-all duration-500">
            <div class="flex justify-between items-start mb-6">
                <div class="w-10 h-10 bg-off-white flex items-center justify-center text-orange-400 rounded-sm">
                    <i class="fa-solid fa-clock text-sm"></i>
                </div>
                <a class="text-[10px] uppercase tracking-widest font-bold text-orange-400 hover:underline" href="<%: ResolveUrl("~/Admin/Orders.aspx?status=pending") %>">Process</a>
            </div>
            <div class="text-[10px] uppercase tracking-widest font-bold text-gray-400 mb-2">Pending Tasks</div>
            <div class="text-4xl font-serif text-text-dark"><asp:Literal ID="litPending" runat="server" /></div>
        </div>

        <!-- Card 4 -->
        <div class="bg-white border-l-4 border-blue-200 p-8 shadow-sm hover:shadow-xl transition-all duration-500">
            <div class="flex justify-between items-start mb-6">
                <div class="w-10 h-10 bg-off-white flex items-center justify-center text-blue-400 rounded-sm">
                    <i class="fa-solid fa-truck text-sm"></i>
                </div>
                <a class="text-[10px] uppercase tracking-widest font-bold text-blue-400 hover:underline" href="<%: ResolveUrl("~/Admin/Orders.aspx?status=delivering") %>">Track</a>
            </div>
            <div class="text-[10px] uppercase tracking-widest font-bold text-gray-400 mb-2">In Transit</div>
            <div class="text-4xl font-serif text-text-dark"><asp:Literal ID="litDelivering" runat="server" /></div>
        </div>
    </div>

    <div class="flex flex-col xl:flex-row gap-12">
        <!-- Latest Orders -->
        <div class="w-full xl:w-2/3">
            <div class="bg-white shadow-sm border border-gray-100 overflow-hidden">
                <div class="px-8 py-6 border-b border-gray-100 flex items-center justify-between bg-off-white/50">
                    <h3 class="text-xs uppercase tracking-widest font-bold text-text-dark">Recent Transactions</h3>
                    <a class="text-[10px] uppercase tracking-widest font-bold text-primary hover:underline" href="<%: ResolveUrl("~/Admin/Orders.aspx") %>">All Orders</a>
                </div>
                <div class="overflow-x-auto">
                    <table class="w-full text-left">
                        <thead>
                            <tr class="text-[10px] uppercase tracking-widest font-bold text-gray-400 border-b border-gray-100 bg-gray-50/50">
                                <th class="px-8 py-4">Ref Code</th>
                                <th class="px-8 py-4">Client</th>
                                <th class="px-8 py-4">Date</th>
                                <th class="px-8 py-4 text-right">Value</th>
                                <th class="px-8 py-4">Status</th>
                                <th class="px-8 py-4"></th>
                            </tr>
                        </thead>
                        <tbody id="latestOrdersBody" runat="server" class="divide-y divide-gray-50">
                            <asp:Literal ID="litLatestOrders" runat="server" />
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Low Stock -->
        <div class="w-full xl:w-1/3">
            <div class="bg-white shadow-sm border border-gray-100 overflow-hidden">
                <div class="px-8 py-6 border-b border-gray-100 flex items-center justify-between bg-off-white/50">
                    <h3 class="text-xs uppercase tracking-widest font-bold text-text-dark">Stock Alerts</h3>
                    <a class="text-[10px] uppercase tracking-widest font-bold text-primary hover:underline" href="<%: ResolveUrl("~/Admin/Products.aspx") %>">Inventory</a>
                </div>
                <div class="p-0">
                    <table class="w-full text-left">
                        <thead>
                            <tr class="text-[10px] uppercase tracking-widest font-bold text-gray-400 border-b border-gray-100 bg-gray-50/50">
                                <th class="px-8 py-4">Product Name</th>
                                <th class="px-8 py-4 text-right">Available</th>
                                <th class="px-8 py-4"></th>
                            </tr>
                        </thead>
                        <tbody class="divide-y divide-gray-50 text-sm">
                            <asp:Literal ID="litLowStock" runat="server" />
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
