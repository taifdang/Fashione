namespace eCommerce_API.DTO
{
    public class ProductImageDTO
    {
        public int id { get; set; }
        public string name { get; set; }    
        public double price { get; set; }
        public double sale { get; set; }
        public string description { get; set; }
        public string? image { get; set; }
        public int categoryId { get; set; }         
        public IFormFile? file { get; set; }
    }
}
