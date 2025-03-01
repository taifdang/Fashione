using eCommerce_API.DTO;
using eCommerce_API.Models;
using eCommerce_API.Translate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
namespace eCommerce_API.Service
{
    public class ProductService : IProductService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ImageService _imageService;
      
        public ProductService(DatabaseContext databaseContext, ImageService imageService)
        {
            this._databaseContext = databaseContext;
            this._imageService = imageService;
           
        }
        public  async Task<Product> Add([FromForm] ProductImageDTO IproducDTO)
        {
            var product = new Product
            { 
                id = IproducDTO.id,
                name = IproducDTO.name,              
                price = IproducDTO.price,
                sale = IproducDTO.sale,
                description = IproducDTO.description,
                //not image
                categoryId = IproducDTO.categoryId,               
            };
            //slug
            product.slug = VietnameseSign.RemoveSigns(IproducDTO.name.ToLower());
            //process image
            if(IproducDTO.file == null) return null;
            if (!GetExtentionFile(IproducDTO.file)) return null;
            product.image = await SaveImage(IproducDTO.file);
            //save product
            try
            {
                _databaseContext.Products.Add(product);
                await _databaseContext.SaveChangesAsync();
            }
            catch
            {
                return null;
            }      
            return product;
            //return result.Entity;
        }
        public async Task Delete(int id)
        {
            var result = await _databaseContext.Products.FirstOrDefaultAsync(x => x.id == id);
            if(result != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", result.image);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _databaseContext.Remove(result);
                await _databaseContext.SaveChangesAsync();
            }
            //                 
        }      
        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            var data = await _databaseContext.Products.Include(x=>x.category).Select(p=>new ProductDTO
            {
                id = p.id,
                name = p.name,
                price = p.price,
                sale = p.sale,
                description = p.description,
                image = _imageService.GetImageUrl(p.image),
                category = p.category.name,
                categoryId = p.categoryId,
            }).ToListAsync();
            return data;
        }
        public bool GetExtentionFile(IFormFile file)
        {
            var listExtention = new string[] { ".png", ".jpg", "gif" };
            return listExtention.Contains(Path.GetExtension(file.FileName))
                ? true : false;
        }
        public async Task<ProductDTO> GetId(int id)
        {
            var data = await _databaseContext.Products.Include(x=>x.category).FirstOrDefaultAsync(p=>p.id==id);
            if (data == null) return null;           
            var get_data = new ProductDTO
            {
            id = data.id,
            name = data.name,
            price = data.price,
            description = data.description,
            image = _imageService.GetImageUrl(data.image),
            category = data.category.name,
            categoryId = data.category.id,
            };
            return get_data;
            
        }
        public int GetPageRanges(int pageSize, int totalItem)
        {
            return totalItem % pageSize > 0 ? totalItem / pageSize + 1 : totalItem / pageSize;
        }
        public int GetTotalProduct(string keyword)
        {
            return string.IsNullOrEmpty(keyword)
                 ? _databaseContext.Products.ToList().Count()
                 : _databaseContext.Products.Where(p => p.slug.Contains(VietnameseSign.RemoveSigns(keyword).ToLower())).Count();
        }
        public async Task<IEnumerable<ProductDTO>?> GetAllPagination(int currentPage, int pageSize)
        {
            int totalItem = GetTotalProduct("");
            int skip = (currentPage - 1) * pageSize;
            int pageRanges = GetPageRanges(pageSize, totalItem);

            if (currentPage <= 0) return null;
            if (currentPage > pageRanges) return null;

            var data = await _databaseContext.Products
                .Include(c => c.category)
                .Select(p => new ProductDTO
                {
                    id = p.id,
                    name = p.name,
                    price = p.price,
                    sale = p.sale,
                    description = p.description,
                    image = _imageService.GetImageUrl(p.image),
                    category = p.category.name,
                    categoryId = p.categoryId,
                })
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            return data;
        }
        public async Task<string> SaveImage(IFormFile file)
        {
            var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
               await file.CopyToAsync(stream);
            }
            return fileName;
        }
        public async Task<IEnumerable<ProductDTO>?> Search(string? keyword, int currentPage, int pageSize)
        {
            string new_keyword = VietnameseSign.RemoveSigns(keyword.ToLower());
                      
            int totalItem = GetTotalProduct(new_keyword);
            int skip = (currentPage - 1) * pageSize;
            int pageRanges = GetPageRanges(pageSize,totalItem);

            if (currentPage <= 0) return null;
            if (currentPage > pageRanges) return null;

            var data = _databaseContext.Products
            .Include(x => x.category)
            .Where(y => y.slug.Contains(new_keyword))
            .Select(p => new ProductDTO
            {
                id = p.id,
                name = p.name,
                price = p.price,
                description = p.description,
                image = _imageService.GetImageUrl(p.image),
                category = p.category.name,
            })
            .Skip(skip)
            .Take(pageSize)
            .ToList();
            return data;      
        }
        public async Task<Product> Update([FromForm] ProductImageDTO productDTO)
        {
            var result = await _databaseContext.Products.FirstOrDefaultAsync(p => p.id == productDTO.id);
            if(result != null)
            {
                result.id = productDTO.id;
                result.name = productDTO.name;           
                result.price = productDTO.price;
                result.sale = productDTO.sale;
                result.description = productDTO.description;
               //result.image
                result.categoryId = productDTO.categoryId;
            };
            //
            //slug
            result.slug = VietnameseSign.RemoveSigns(productDTO.name.ToLower());
            if (productDTO.file == null) return null;
            if(!GetExtentionFile(productDTO.file)) return null;
            //delete old image
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", result.image);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            //save new image
            try
            {
                result.image = await SaveImage(productDTO.file);
                await _databaseContext.SaveChangesAsync();
            }
            catch
            {
                return null;
            }
               
            return result;
           

        }
       
    }
}
