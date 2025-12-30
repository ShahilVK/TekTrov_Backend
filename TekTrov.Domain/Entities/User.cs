using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekTrov.Domain.Entities
{
    //public class User
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Email { get; set; }

    //    public string Password { get; set; } // HASHED

    //    public string Role { get; set; }

    //    public string? RefreshToken { get; set; }
    //    public DateTime? RefreshTokenExpiryTime { get; set; }

    //    public ICollection<Wishlist> Wishlists { get; set; }

    //    public ICollection<Cart> Carts { get; set; } = new List<Cart>();

    //    public ICollection<Order> Orders { get; set; } = new List<Order>();



    //}


    public class User : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; // hashed
        public string Role { get; set; } = "User";

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
