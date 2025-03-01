using eCommerce_API.DTO;
using eCommerce_API.Models;
namespace eCommerce_API.Service
{
    public interface ICategoryService
    {       
        Task<CategoryDTO> GetId(int id, int currentPage, int pageSize);
        Task<IEnumerable<CategoryDTO>> GetAll();      
        Task<Category> Add(Category id); 
        Task<Category> Update(Category id);
        Task Delete(int id);
        Task<IEnumerable<ProductDTO>> Search(int? id, int currentPage,int pageSize);
        int GetTotalProduct(string? keyword);
        int GetPageRanges(int pageSize, int totalItem);
    }
}
