namespace eCommerce_API.DTO
{
    public class CategoryTypeDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<CategoryDTO> categories { get; set; }
    }
}
