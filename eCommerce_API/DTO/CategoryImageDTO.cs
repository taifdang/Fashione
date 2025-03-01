namespace eCommerce_API.DTO
{
    public class CategoryImageDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? image { get; set; }
        public IFormFile? file { get; set; }
    }
}
