using eCommerce_API.DTO;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Mvc;
namespace eCommerce_API.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAll();
        Task<ProductDTO> GetId(int id);
        Task<IEnumerable<ProductDTO>> GetAllPagination(int currentPage,int pageSize);
       
        Task<IEnumerable<ProductDTO>> Search(string? keyword, int currentPage, int pageSize);
        Task Delete(int id);
        Task<Product> Add([FromForm]ProductImageDTO IproducDTO);
        Task<Product> Update([FromForm] ProductImageDTO productDTO);
      
        Task<string> SaveImage(IFormFile file);
        bool GetExtentionFile(IFormFile file);
        int GetTotalProduct(string? keyword);
        int GetPageRanges(int pageSize, int totalItem);      
    }
}
