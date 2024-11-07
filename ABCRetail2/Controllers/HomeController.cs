using ABCRetail2.Data;
using ABCRetail2.Models;
using ABCRetail2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ABCRetail2.Controllers
{
    public class HomeController : Controller
    {
        private readonly RetailContext _context;

        public HomeController(RetailContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ViewProducts()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Checkout(string partitionKey, string rowKey)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.PartitionKey == partitionKey && p.RowKey == rowKey);

            if (product == null) return NotFound();

            // Create a new cart item based on the selected product
            var cartItem = new CartItem
            {
                ProductName = product.Name,
                ProductPrice = product.Price,
                ProductImageUri = product.ImageUri,
                Quantity = 1,
                ProductPartitionKey = product.PartitionKey,
                ProductRowKey = product.RowKey
            };

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = new List<CartItem> { cartItem },
                CustomerName = HttpContext.Session.GetString("CustomerName"),
                CustomerEmail = HttpContext.Session.GetString("UserEmail")
            };

            return View(checkoutViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(
            string[] partitionKeys, string[] rowKeys,
            string customerName, string customerEmail, string address,
            string city, string zipCode, string country, string province)
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            var cartItems = await _context.CartItems
                .Where(c => c.PartitionKey == userId).ToListAsync();

            var orders = new List<Order>();
            for (int i = 0; i < partitionKeys.Length; i++)
            {
                var cartItem = cartItems
                    .FirstOrDefault(c => c.ProductPartitionKey == partitionKeys[i] && c.ProductRowKey == rowKeys[i]);

                if (cartItem != null)
                {
                    var order = new Order
                    {
                        PartitionKey = customerEmail,
                        RowKey = Guid.NewGuid().ToString(),
                        CustomerName = customerName,
                        CustomerEmail = customerEmail,
                        Address = address,
                        City = city,
                        ZipCode = zipCode,
                        Country = country,
                        Province = province,
                        ProductName = cartItem.ProductName,
                        ProductPrice = cartItem.ProductPrice,
                        ProductImageUri = cartItem.ProductImageUri
                    };

                    _context.Orders.Add(order);
                    orders.Add(order);

                    var orderMessage = new OrderMessage
                    {
                        CustomerEmail = customerEmail,
                        ProductName = cartItem.ProductName,
                        ProductPrice = cartItem.ProductPrice
                    };

                    // Process orderMessage logic here if needed
                }
            }

            await _context.SaveChangesAsync();
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return View("OrderConfirmation", orders);
        }

        public async Task<IActionResult> OrderConfirmation(string orderId)
        {
            var orders = await _context.Orders
                .Where(o => o.RowKey == orderId).ToListAsync();

            if (!orders.Any()) return NotFound("Order not found.");
            return View(orders);
        }

        public async Task<IActionResult> ViewOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            return View(orders);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(IFormFile file, string name, string description, string price)
        {
            // Validate and parse the price input
            if (!decimal.TryParse(price, out decimal parsedPrice))
            {
                ModelState.AddModelError("Price", "Invalid price format.");
                return View();
            }

            string imageUri = string.Empty;

            // Save the uploaded file to wwwroot/uploads directory
            if (file != null && file.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                imageUri = "/uploads/" + uniqueFileName; // Store the relative path
            }
            else
            {
                ModelState.AddModelError("File", "Please upload a valid product image.");
                return View();
            }

            // Create and save the new product entity
            var product = new Product
            {
                PartitionKey = "Product",
                RowKey = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Price = (int)parsedPrice,  // Cast decimal to int
                ImageUri = imageUri
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageProducts");
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                profile.PartitionKey = profile.CustomerEmail.Substring(0, 1).ToUpper();
                profile.RowKey = Guid.NewGuid().ToString();

                _context.CustomerProfiles.Add(profile);
                await _context.SaveChangesAsync();

                return RedirectToAction("CustomerProfiles");
            }

            return View("AddCustomerProfile", profile); // Reload form with validation errors if any
        }

        public async Task<IActionResult> CustomerProfiles()
        {
            var profiles = await _context.CustomerProfiles.ToListAsync();
            return View(profiles);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string partitionKey, string rowKey)
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Home");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.PartitionKey == partitionKey && p.RowKey == rowKey);

            if (product == null) return NotFound();

            var cartItem = new CartItem
            {
                PartitionKey = userId,
                RowKey = Guid.NewGuid().ToString(),
                ProductName = product.Name,
                ProductPrice = product.Price,
                ProductImageUri = product.ImageUri,
                ProductRowKey = product.RowKey,
                ProductPartitionKey = product.PartitionKey
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> ViewCart()
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            var cartItems = await _context.CartItems
                .Where(c => c.PartitionKey == userId).ToListAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(string rowKey)
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.PartitionKey == userId && c.RowKey == rowKey);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartQuantity(string rowKey, string action)
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.PartitionKey == userId && c.RowKey == rowKey);

            if (cartItem != null)
            {
                if (action == "increase")
                {
                    cartItem.Quantity += 1;
                }
                else if (action == "decrease" && cartItem.Quantity > 1)
                {
                    cartItem.Quantity -= 1;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> CheckoutCart()
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            var cartItems = await _context.CartItems
                .Where(c => c.PartitionKey == userId).ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("ViewCart");
            return View("Checkout", cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> UploadContractFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Define the path to save the file
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "contracts");
                Directory.CreateDirectory(uploadsFolder);  // Ensure the folder exists

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Optionally, save file information in the database (e.g., file name, path, upload date)

                // Redirect or display success message after upload
                return RedirectToAction("Contracts");
            }

            ModelState.AddModelError("file", "Please select a file to upload.");
            return View("Contracts");
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product updatedProduct, IFormFile file)
        {
            // Retrieve the existing product from the database
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.PartitionKey == updatedProduct.PartitionKey && p.RowKey == updatedProduct.RowKey);

            if (product == null) return NotFound();

            // Update product properties
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            // If a new image file is uploaded, replace the existing image
            if (file != null && file.Length > 0)
            {
                // Define path for uploads
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                // Generate a unique file name and save the file
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save new image file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Delete the old image if one exists
                if (!string.IsNullOrEmpty(product.ImageUri))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUri.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Update ImageUri to new file path
                product.ImageUri = "/uploads/" + uniqueFileName;
            }

            // Save changes to the database
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageProducts");
        }
    }
}
