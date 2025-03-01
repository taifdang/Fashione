using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce_API.Models
{
    public class Product
    {      
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public double price { get; set; }
        public double sale { get; set; }
        public string description { get; set; }
        public string? image { get; set; }
        public int? categoryId { get; set; }      
        public Category? category { get; set; }      
      
    }
}
