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
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = null!;
        public int Stock { get; set; }


        public double Rating { get; set; } = 0;
        public int SoldCount { get; set; } = 0;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;


    }
}
