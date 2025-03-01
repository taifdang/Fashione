using eCommerce_API.DTO;
using eCommerce_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCommerce_API.Service
{
    public class CategoryService : ICategoryService
    {
        public readonly DatabaseContext _databaseContext;
        public readonly ImageService _imageService;
        public CategoryService(DatabaseContext databaseContext,ImageService imageService)
        {
            this._databaseContext = databaseContext;
            this._imageService = imageService;
        }
        public Task<Category> Add(Category id)
        {
            throw new NotImplementedException();
        }
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            var data = await _databaseContext.Categories
             .Include(x => x.products)
             .Select(c=>new CategoryDTO
             {
                 id = c.id,
                 name = c.name,
                
                 products = c.products.Select(p=>new ProductDTO
                 {
                     id = p.id,
                     name = p.name,               
                     price = p.price,
                     sale = p.sale,
                     description = p.description,
                     image = _imageService.GetImageUrl(p.image),   
                     category = p.category.name,
                     categoryId = p.categoryId,
                 }).ToList()
             })
             .ToListAsync();
            return data;
        }

        public async Task<CategoryDTO> GetId(int id, int currentPage, int pageSize)
        {
            //pagination          
            
            int totalItem =  _databaseContext.Products.Where(p => p.categoryId == id).Count();
            int skip = (currentPage - 1) * pageSize;
            int pageRanges = GetPageRanges(pageSize,totalItem);

            //get data
            var data = await _databaseContext.Categories.Include(c => c.products).FirstOrDefaultAsync(x => x.id == id);                                  
            if (data == null) return null;               
            var result = new CategoryDTO
            {
                id = data.id,
                name = data.name,
                products = data.products.Select(p => new ProductDTO
                {
                    id = p.id,
                    name = p.name,
                    price = p.price,
                    description = p.description,
                    image = _imageService.GetImageUrl(p.image),
                    category = p.category.name,
                    categoryId = p.category.id,

                }).Skip(skip).Take(pageSize).ToList()
            };
            return result;
        }
        public async Task<IEnumerable<ProductDTO>> Search(int? id, int currentPage,int pageSize)
        {          
            
            int totalItem = _databaseContext.Products.Where(p=>p.categoryId == id).Count();
            int skip = (currentPage - 1) * pageSize;
            int pageRanges = GetPageRanges(pageSize, totalItem);
            if(currentPage <= pageRanges && currentPage > 0) {
                var data = _databaseContext.Products
                    .Include(x => x.category)
                    .Where(y => y.category.id == id)
                    .Select(
                        p => new ProductDTO
                        {
                            id = p.id,
                            name = p.name,
                            price = p.price,
                            description = p.description,
                            image = _imageService.GetImageUrl(p.image),
                            category = p.category.name
                        }).Skip(skip).Take(pageSize).ToList();
                    
                return data;
            }
            return null;
        }

        public Task<Category> Update(Category id)
        {
            throw new NotImplementedException();
        }
        #region NoAction      
        public int GetTotalProduct(string? keyword)
        {
            return _databaseContext.Products.Where(p => p.category.name.Contains(keyword)).Count();
        }

        public int GetPageRanges(int pageSize, int totalItem)
        {
            return totalItem % pageSize > 0 ? totalItem / pageSize + 1 : totalItem / pageSize;
        }
        #endregion

    }
}
