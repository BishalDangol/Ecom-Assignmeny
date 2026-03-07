using System;
using System.Collections.Generic;
using Saja.DAL;
using Saja.Entities;

namespace Saja.BLL
{
    /// <summary>
    /// Implements business rules for Products.
    /// </summary>
    public class ProductBLL
    {
        private ProductDAL productDAL;
        private VendorDAL vendorDAL;

        public ProductBLL()
        {
            productDAL = new ProductDAL();
            vendorDAL = new VendorDAL();
        }

        public List<Product> GetAllProducts()
        {
            return productDAL.GetAll();
        }

        public Product GetProductById(int id)
        {
            if (id <= 0) return null;
            return productDAL.GetById(id);
        }

        public List<Product> SearchProducts(string keyword, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            return productDAL.Search(keyword, categoryId, minPrice, maxPrice);
        }

        /// <summary>
        /// Adds a new product with business validation and vendor verification check.
        /// </summary>
        public OperationResult AddProduct(Product product)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(product.Name))
                return OperationResult.Failed("Product name is required.");
            
            if (product.Price <= 0)
                return OperationResult.Failed("Price must be greater than zero.");

            if (product.Stock < 0)
                return OperationResult.Failed("Initial stock cannot be negative.");

            // Nepal Market Rule: Check if Vendor is verified
            Vendor vendor = vendorDAL.GetById(product.VendorId);
            if (vendor == null)
                return OperationResult.Failed("Invalid Vendor.");
            
            if (!vendor.Verified)
                return OperationResult.Failed("Vendor must be verified before listing products.");

            int newId = productDAL.Insert(product);
            if (newId > 0)
                return OperationResult.Succeeded("Product added successfully.", newId);
            
            return OperationResult.Failed("Could not save product to database.");
        }

        /// <summary>
        /// Checks if enough stock is available.
        /// </summary>
        public bool IsStockAvailable(int productId, int requestedQuantity)
        {
            Product p = productDAL.GetById(productId);
            return p != null && p.Stock >= requestedQuantity;
        }
    }
}
