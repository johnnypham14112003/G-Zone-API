using GZone.Repository;
using GZone.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Asp.Versioning;

namespace GZone.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AppTestSeedingController : ControllerBase
    {
        private readonly GZoneDbContext _context;

        public AppTestSeedingController(GZoneDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("seed-my-warranty-data")]
        public async Task<IActionResult> SeedWarrantyData()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("User is not authenticated correctly.");
            }

            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = "Test Helmets " + DateTime.Now.Second,
                Description = "Category for testing",
                Slug = "test-helmets-" + DateTime.Now.Ticks,
                IsActive = true,
                ImageUrl = ""
            };
            
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                CategoryId = category.CategoryId,
                ProductName = "AGV Pista GP RR (Test Data)",
                Sku = "AGV-TEST-" + DateTime.Now.Ticks,
                Description = "A tested helmet for warranty",
                BasePrice = 1500000,
                Brand = "AGV",
                Material = "Carbon Fiber",
                Specifications = "DOT and ECE Certified",
                Weight = 1.45m,
                Dimension = "L",
                IsActive = true,
                ImageUrl = "https://product.hstatic.net/200000383186/product/agv_pista_gp_rr_futuro_1_e41753c15aa345ebaf8f4dcca2d8b560_1024x1024.jpg",
                WarrantyPeriodMonths = 12
            };

            var variant = new ProductVariant
            {
                VariantId = Guid.NewGuid(),
                ProductId = product.ProductId,
                Sku = "AGV-TEST-L",
                Color = "Carbon Black",
                Size = "L",
                AdditionalPrice = 0,
                StockQuantity = 10
            };

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = userId,
                OrderNumber = "ORD-TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Status = "Delivered",
                Subtotal = 1500000,
                TotalAmount = 1500000,
                PaymentMethod = "COD",
                PaymentStatus = "Paid",
                ReceiverName = "Tester",
                ReceiverPhone = "0987654321",
                ShippingAddress = "123 Test St",
                ShippingCity = "Test City",
                ShippingDistrict = "Test District",
                ShippingWard = "Test Ward"
            };

            var orderDetail = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid(),
                OrderId = order.OrderId,
                ProductVariantId = variant.VariantId,
                ProductName = product.ProductName,
                VariantInfo = "Color: Carbon Black, Size: L",
                Quantity = 1,
                UnitPrice = 1500000,
                TotalPrice = 1500000,
                WarrantyPeriodMonths = 12,
                Status = "Valid",
                CustomDesignImage = "",
                CustomDesignNote = ""
            };

            _context.Categories.Add(category);
            _context.Products.Add(product);
            _context.ProductVariants.Add(variant);
            _context.Orders.Add(order);
            _context.OrderDetails.Add(orderDetail);

            await _context.SaveChangesAsync();

            return Ok(new { 
                statusCode = 200,
                message = "Tạo dữ liệu test thành công!", 
                data = new {
                    OrderNumber = order.OrderNumber,
                    ProductName = product.ProductName
                }
            });
        }
    }
}