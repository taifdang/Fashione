using eCommerce_API.Models;

namespace eCommerce_API.DTO
{
    public class ProductDTO
    {
        public int id { get; set; }
        public string name { get; set; }       
        public double price { get; set; }
        public double sale { get; set; }
        public string description { get; set; }
        public string? image { get; set; }           
        public string category { get; set; }
        public int? categoryId { get; set; }

    }
}
