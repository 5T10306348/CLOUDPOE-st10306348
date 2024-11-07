using System.Collections.Generic;
using ABCRetail2.Models;

namespace ABCRetail2.ViewModels
{
    public class CheckoutViewModel
    {
        // List of items in the cart for checkout
        public List<CartItem> CartItems { get; set; }

        // Customer details for the checkout
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }

        // Total price for the order
        public decimal TotalPrice => CalculateTotalPrice();

        // Calculate the total price based on CartItems
        private decimal CalculateTotalPrice()
        {
            decimal total = 0;
            if (CartItems != null)
            {
                foreach (var item in CartItems)
                {
                    total += item.ProductPrice * item.Quantity;
                }
            }
            return total;
        }
    }
}
