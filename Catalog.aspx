<%@ Page Language="C#" MasterPageFile="~/MasterPages/Site.master"
    AutoEventWireup="true" CodeFile="Catalog.aspx.cs"
    Inherits="serena.Site.CatalogPage" %>

<asp:Content ID="t" ContentPlaceHolderID="TitleContent" runat="server">Shop Our Collection | Saja</asp:Content>

<asp:Content ID="m" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- Hero Header for Catalog -->
    <section class="bg-off-white py-12 mb-12">
        <div class="container mx-auto px-4 lg:px-8">
            <h1 class="text-4xl font-serif mb-2">Shop Collection</h1>
            <nav class="flex text-xs uppercase tracking-widest text-gray-400">
                <a runat="server" href="~/Default.aspx" class="hover:text-primary transition-colors">Home</a>
                <span class="mx-2">/</span>
                <span class="text-text-dark">Shop</span>
            </nav>
        </div>
    </section>

    <div class="container mx-auto px-4 lg:px-8 pb-24">
        <div class="flex flex-col lg:flex-row gap-12">

            <!-- Filters Sidebar -->
            <aside class="w-full lg:w-1/4 space-y-10">
                <!-- Search -->
                <div>
                    <h4 class="text-xs uppercase tracking-[0.2em] font-bold mb-6">Search</h4>
                    <div class="relative">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="w-full bg-white border border-gray-100 px-4 py-3 text-sm focus:border-primary focus:outline-none transition-colors" placeholder="Type to search..."></asp:TextBox>
                        <i class="fa-solid fa-magnifying-glass absolute right-4 top-1/2 -translate-y-1/2 text-gray-300"></i>
                    </div>
                </div>

                <!-- Categories -->
                <div>
                    <h4 class="text-xs uppercase tracking-[0.2em] font-bold mb-6">Categories</h4>
                    <asp:RadioButtonList ID="rblCategories" runat="server" CssClass="space-y-4 text-sm text-gray-500" RepeatLayout="UnorderedList"></asp:RadioButtonList>
                </div>

                <!-- Price Range -->
                <div>
                    <h4 class="text-xs uppercase tracking-[0.2em] font-bold mb-6">Price Range</h4>
                    <div class="flex items-center gap-4">
                        <asp:TextBox ID="txtPriceMin" runat="server" CssClass="w-full bg-white border border-gray-100 px-4 py-3 text-sm focus:border-primary focus:outline-none text-center" placeholder="Min"></asp:TextBox>
                        <span class="text-gray-300">—</span>
                        <asp:TextBox ID="txtPriceMax" runat="server" CssClass="w-full bg-white border border-gray-100 px-4 py-3 text-sm focus:border-primary focus:outline-none text-center" placeholder="Max"></asp:TextBox>
                    </div>
                </div>

                <!-- Filter Actions -->
                <div class="flex flex-col gap-4">
                    <asp:Button ID="btnApply" runat="server" Text="Apply Filters" CssClass="bg-primary text-white text-xs uppercase tracking-widest font-bold py-4 hover:bg-primary/90 transition-all cursor-pointer" OnClick="btnApply_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear All" CssClass="border border-gray-100 text-text-dark text-xs uppercase tracking-widest font-bold py-4 hover:bg-off-white transition-all cursor-pointer" OnClick="btnClear_Click" />
                </div>
            </aside>

            <!-- Products section -->
            <section class="flex-grow">
                <!-- Toolbar -->
                <div class="flex items-center justify-between mb-10 border-b border-gray-100 pb-6">
                    <p class="text-sm text-gray-400 uppercase tracking-widest">Showing all results</p>
                    <div class="flex items-center gap-4">
                        <span class="text-xs uppercase tracking-widest text-gray-400">Sort:</span>
                        <select class="text-xs uppercase tracking-widest font-bold focus:outline-none">
                            <option>Default Sorting</option>
                            <option>Price: Low to High</option>
                            <option>Price: High to Low</option>
                            <option>Newest Arrivals</option>
                        </select>
                    </div>
                </div>

                <asp:ListView ID="lvProducts" runat="server"
                    OnItemCommand="lvProducts_ItemCommand"
                    OnPagePropertiesChanging="lvProducts_PagePropertiesChanging">
                    <LayoutTemplate>
                        <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-x-8 gap-y-12">
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                        </div>
                    </LayoutTemplate>

                    <ItemTemplate>
                        <div class="group">
                            <!-- Image Container -->
                            <div class="relative aspect-[3/4] overflow-hidden bg-off-white mb-6">
                                <a href='<%# GetProductUrl(Eval("name")) %>'>
                                    <img class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
                                         src='<%# GetProductImageUrl(Eval("image")) %>'
                                         alt='<%# Html(Eval("name")) %>'
                                         onerror="this.onerror=null;this.src='https://via.placeholder.com/600x800?text=No+Image';" />
                                </a>
                                
                                <!-- Hover Actions -->
                                <div class="absolute inset-x-0 bottom-0 p-4 translate-y-full group-hover:translate-y-0 transition-transform duration-500 bg-white/90 backdrop-blur-sm border-t border-gray-100">
                                    <div class="flex items-center gap-2">
                                        <div class="flex items-center border border-gray-200 h-10">
                                            <asp:Button ID="btnMinus" runat="server" Text="-" CommandName="Decrement"
                                                CommandArgument='<%# Eval("id") %>' CssClass="w-8 h-full flex items-center justify-center hover:bg-off-white transition-colors cursor-pointer" />
                                            <asp:TextBox ID="txtQty" runat="server" Text="1" CssClass="w-10 h-full text-center text-xs focus:outline-none" />
                                            <asp:Button ID="btnPlus" runat="server" Text="+" CommandName="Increment"
                                                CommandArgument='<%# Eval("id") %>' CssClass="w-8 h-full flex items-center justify-center hover:bg-off-white transition-colors cursor-pointer" />
                                        </div>
                                        <asp:LinkButton ID="btnAdd" runat="server"
                                              CommandName="AddToCart" CommandArgument='<%# Eval("id") %>'
                                              CssClass="flex-grow bg-primary text-white text-[10px] uppercase tracking-widest font-bold h-10 flex items-center justify-center hover:bg-primary/90 transition-colors"
                                              Enabled='<%# Convert.ToInt32(Eval("stock")) > 0 %>'>
                                            <i class="fa-solid fa-cart-shopping mr-2"></i> Add To Cart
                                        </asp:LinkButton>
                                    </div>
                                </div>

                                <asp:Literal ID="litSoldOut" runat="server" Visible='<%# Convert.ToInt32(Eval("stock")) <= 0 %>'>
                                    <div class="absolute top-4 left-4 bg-text-dark text-white text-[10px] uppercase tracking-widest px-3 py-1">Sold Out</div>
                                </asp:Literal>
                            </div>

                            <!-- Info -->
                            <div class="text-center">
                                <div class="text-[10px] uppercase tracking-[0.2em] text-gray-400 mb-2">
                                     <%# Html(Eval("category_name")) %>
                                </div>
                                <h3 class="font-serif text-lg mb-2">
                                    <a href='<%# GetProductUrl(Eval("name")) %>' class="hover:text-primary transition-colors">
                                        <%# Html(Eval("name")) %>
                                    </a>
                                </h3>
                                <div class="font-medium text-primary tracking-wide">RS <%# Convert.ToDecimal(Eval("price")).ToString("N2") %></div>
                            </div>
                        </div>
                    </ItemTemplate>

                    <EmptyDataTemplate>
                        <div class="py-20 text-center text-gray-400">
                            <i class="fa-solid fa-magnifying-glass text-4xl mb-4 opacity-20"></i>
                            <p class="uppercase tracking-widest text-sm">No products matched your filters.</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>

                <!-- Custom Styled Pagination -->
                <div class="mt-20 flex justify-center border-t border-gray-100 pt-10">
                    <asp:DataPager ID="pager" runat="server" PagedControlID="lvProducts" PageSize="12" QueryStringField="page" CssClass="flex gap-2">
                        <Fields>
                            <asp:NextPreviousPagerField ShowFirstPageButton="false" ShowLastPageButton="false"
                                ShowPreviousPageButton="true" ShowNextPageButton="false"
                                ButtonType="Link" PreviousPageText="Prev"
                                ButtonCssClass="px-4 py-2 text-[10px] uppercase tracking-widest font-bold border border-gray-200 hover:bg-off-white transition-colors" />
                            <asp:NumericPagerField ButtonCount="5" ButtonType="Link"
                                NumericButtonCssClass="px-4 py-2 text-[10px] uppercase tracking-widest font-bold border border-gray-200 hover:bg-off-white transition-colors"
                                CurrentPageLabelCssClass="px-4 py-2 text-[10px] uppercase tracking-widest font-bold bg-primary text-white border border-primary" />
                            <asp:NextPreviousPagerField ShowFirstPageButton="false" ShowLastPageButton="false"
                                ShowPreviousPageButton="false" ShowNextPageButton="true"
                                ButtonType="Link" NextPageText="Next"
                                ButtonCssClass="px-4 py-2 text-[10px] uppercase tracking-widest font-bold border border-gray-200 hover:bg-off-white transition-colors" />
                        </Fields>
                    </asp:DataPager>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
