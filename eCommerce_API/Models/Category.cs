using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce_API.Models
{
    public class Category
    {      
        public int id { get; set; }
        public string name { get; set; }  
        public string? image {  get; set; }
        public int? categoryTypeId { get; set; }
        public  CategoryType? categoryType { get; set; }
        public ICollection<Product> products { get; set; }
    }
}
