using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekTrov.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public ICollection<Wishlist> Wishlists { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();


    }
}
