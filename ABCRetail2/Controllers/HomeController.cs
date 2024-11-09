using ABCRetail2.Data;
using ABCRetail2.Models;
using ABCRetail2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Contracts()
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
            string customerName, string address, string city, string zipCode, string country, string province)
        {
            var userId = HttpContext.Session.GetString("UserEmail");
            var commonPartitionKey = Guid.NewGuid().ToString(); // Use a unique key for each order session

            var cartItems = await _context.CartItems
                .Where(c => partitionKeys.Contains(c.ProductPartitionKey) && rowKeys.Contains(c.ProductRowKey))
                .ToListAsync();

            foreach (var cartItem in cartItems)
            {
                var order = new Order
                {
                    PartitionKey = commonPartitionKey, // Assign a common key for this order session
                    RowKey = Guid.NewGuid().ToString(),
                    CustomerName = customerName,
                    CustomerEmail = userId,
                    Address = address,
                    City = city,
                    ZipCode = zipCode,
                    Country = country,
                    Province = province,
                    ProductName = cartItem.ProductName,
                    ProductPrice = cartItem.ProductPrice,
                    ProductImageUri = cartItem.ProductImageUri,
                    Quantity = cartItem.Quantity
                };

                _context.Orders.Add(order);
            }

            await _context.SaveChangesAsync();
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { partitionKey = commonPartitionKey });
        }

        public async Task<IActionResult> OrderConfirmation(string partitionKey)
        {
            var orders = await _context.Orders
                .Where(o => o.PartitionKey == partitionKey).ToListAsync();

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

        public async Task<IActionResult> CustomerProfiles()
        {
            var profiles = await _context.CustomerProfiles.ToListAsync();
            return View(profiles); // Pass all profiles to the view
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.CustomerProfiles.Add(profile);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Profile saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving profile: " + ex.Message);
                }

                return RedirectToAction("CustomerProfiles");
            }

            var profiles = await _context.CustomerProfiles.ToListAsync();
            return View("CustomerProfiles", profiles); // Reload the view with current data
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
                .Where(c => c.PartitionKey == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("ViewCart");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                CustomerName = HttpContext.Session.GetString("UserName"),
                CustomerEmail = userId // Automatically set the email from the session
            };

            return View("Checkout", checkoutViewModel);
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

                // Create a new Contract instance with the file details
                var contract = new Contract
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    UploadDate = DateTime.Now
                };

                // Save contract information to the database
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();

                // Redirect to display success message after upload
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
        public async Task<IActionResult> ManageProducts()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> EditProduct(string partitionKey, string rowKey)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.PartitionKey == partitionKey && p.RowKey == rowKey);
            return View(product);
        }

        [HttpGet]  // Allow GET for this action
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.PartitionKey == partitionKey && p.RowKey == rowKey);

            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageProducts");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserAccount());
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserAccount user)
        {
            if (ModelState.IsValid)
            {
                // Normalize email to lowercase
                user.Email = user.Email.ToLower();

                // Hash password
                user.Password = HashPassword(user.Password);

                _context.UserAccounts.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(user); // Return view with validation errors if any
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Normalize email to lowercase
            email = email.ToLower();

            // Find user by email
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                Console.WriteLine("User not found with email: " + email);
                return View();
            }

            // Log the retrieved hashed password for debugging
            Console.WriteLine("Retrieved hashed password from DB: " + user.Password);

            // Hash the input password and compare it with the stored hash
            var hashedInputPassword = HashPassword(password);
            Console.WriteLine("Hashed input password: " + hashedInputPassword);

            if (user.Password != hashedInputPassword)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                Console.WriteLine("Password does not match.");
                return View();
            }

            // Set session data for the user
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.Username);
            Console.WriteLine("Login successful, redirecting to Index.");

            return RedirectToAction("Index", "Home");
        }

        // A method to hash the password consistently
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ViewMyOrders()
        {
            // Retrieve the logged-in user's email from the session
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "Home");
            }

            // Fetch orders associated with the logged-in user's email
            var userOrders = await _context.Orders
                .Where(order => order.CustomerEmail == userEmail)
                .ToListAsync();

            // Pass the user's orders to the view
            return View(userOrders);
        }

    }
}
