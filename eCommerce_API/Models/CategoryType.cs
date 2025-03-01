using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce_API.Models
{
    public class CategoryType
    {     
        public int id { get; set; }
        public string name { get; set; }
        public  ICollection<Category> category { get; set; }
    }
}
