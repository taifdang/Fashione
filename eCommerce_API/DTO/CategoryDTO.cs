namespace eCommerce_API.DTO
{
    public class CategoryDTO
    {
        public int id { get; set; }
        public string name { get; set; }       
     
        public List<ProductDTO> products { get; set; }
    }
}
